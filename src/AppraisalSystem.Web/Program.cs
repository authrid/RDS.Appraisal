using System.Security.Claims;
using AppraisalSystem.Application;
using AppraisalSystem.Infrastructure;
using AppraisalSystem.Infrastructure.Persistence;
using AppraisalSystem.Web;
using AppraisalSystem.Web.Authentication;
using AppraisalSystem.Web.Components;
using AppraisalSystem.Web.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

AppDomain.CurrentDomain.UnhandledException += (_, args) =>
{
    if (args.ExceptionObject is Exception ex)
    {
        Console.Error.WriteLine($"[FATAL][AppDomain] {ex}");
    }
    else
    {
        Console.Error.WriteLine("[FATAL][AppDomain] Unknown unhandled exception object.");
    }
};

TaskScheduler.UnobservedTaskException += (_, args) =>
{
    Console.Error.WriteLine($"[FATAL][TaskScheduler] {args.Exception}");
    args.SetObserved();
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var razorComponents = builder.Services.AddRazorComponents();
if (builder.Environment.IsDevelopment())
{
    razorComponents.AddInteractiveServerComponents(options => options.DetailedErrors = true);
}
else
{
    razorComponents.AddInteractiveServerComponents();
}
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = AppRoutes.Account.Login;
        options.AccessDeniedPath = AppRoutes.Account.AccessDenied;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services
    .AddOptions<ThemeOptions>()
    .Bind(builder.Configuration.GetSection(ThemeOptions.SectionName))
    .Validate(ThemeOptionsValidator.IsValid,
        "Theme color configuration is invalid. Use hex format (#RGB, #RGBA, #RRGGBB, or #RRGGBBAA).")
    .ValidateOnStart();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StartupDiagnostics");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(AppRoutes.SystemPages.Error, createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            if (feature?.Error is not null)
            {
                logger.LogError(feature.Error, "Unhandled development exception on path {Path}", context.Request.Path);
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Unhandled exception. Check server logs for details.");
        });
    });
}
app.UseStatusCodePagesWithReExecute(AppRoutes.Account.NotFound, createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost(AppRoutes.Account.LoginPost, async (HttpContext context, IConfiguration configuration) =>
    {
        var form = await context.Request.ReadFormAsync();
        var username = form["username"].ToString();
        var password = form["password"].ToString();
        var returnUrl = form["returnUrl"].ToString();

        var users = configuration.GetSection("Authentication:Users").Get<List<AppUserConfig>>() ?? [];
        var user = users.FirstOrDefault(x =>
            string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase) &&
            x.Password == password);

        if (user is null)
        {
            var loginPath = BuildPathBasePath(context, AppRoutes.Account.Login);
            context.Response.Redirect($"{loginPath}?error=1&returnUrl={Uri.EscapeDataString(returnUrl)}");
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Username),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var safeReturnUrl = NormalizeReturnUrl(returnUrl, context.Request.PathBase, AppRoutes.Dashboard.Main);
        context.Response.Redirect(safeReturnUrl);
    })
    .DisableAntiforgery();

app.MapPost(AppRoutes.Account.LogoutPost, async (HttpContext context) =>
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect(BuildPathBasePath(context, AppRoutes.Account.Login));
    })
    .DisableAntiforgery();

static string BuildPathBasePath(HttpContext context, string appPath)
{
    var normalizedAppPath = appPath.StartsWith("/", StringComparison.Ordinal) ? appPath : $"/{appPath}";
    return context.Request.PathBase.HasValue
        ? $"{context.Request.PathBase}{normalizedAppPath}"
        : normalizedAppPath;
}

static string NormalizeReturnUrl(string? returnUrl, PathString pathBase, string fallbackAppPath)
{
    var normalizedFallback = fallbackAppPath.StartsWith("/", StringComparison.Ordinal)
        ? fallbackAppPath
        : $"/{fallbackAppPath}";

    var fallback = pathBase.HasValue
        ? $"{pathBase}{normalizedFallback}"
        : normalizedFallback;

    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        return fallback;
    }

    if (Uri.TryCreate(returnUrl, UriKind.Absolute, out _)
        || returnUrl.StartsWith("//", StringComparison.Ordinal)
        || !returnUrl.StartsWith("/", StringComparison.Ordinal))
    {
        return fallback;
    }

    if (!pathBase.HasValue)
    {
        return returnUrl;
    }

    return returnUrl.StartsWith(pathBase.Value!, StringComparison.OrdinalIgnoreCase)
        ? returnUrl
        : $"{pathBase}{returnUrl}";
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppraisalDbContext>();
        await DbInitializer.InitializeAsync(dbContext);
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database initialization failed at startup.");
        throw;
    }
}

app.Run();
