using AppraisalSystem.Application.Interfaces;
using AppraisalSystem.Infrastructure.Options;
using AppraisalSystem.Infrastructure.Persistence;
using AppraisalSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppraisalSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<AppraisalDbContext>(options =>
        {
            var provider = dbOptions.Provider.Trim().ToLowerInvariant();

            if (provider == "sqlserver")
            {
                options.UseSqlServer(dbOptions.ConnectionString);
                return;
            }

            if (provider == "postgresql" || provider == "npgsql" || provider == "postgres")
            {
                options.UseNpgsql(dbOptions.ConnectionString);
                return;
            }

            if (provider == "sqlite")
            {
                options.UseSqlite(dbOptions.ConnectionString);
                return;
            }

            throw new InvalidOperationException(
                $"Unsupported database provider '{dbOptions.Provider}'. Use SqlServer, PostgreSQL, or Sqlite.");
        });

        services.AddScoped<IAppraisalRepository, AppraisalRepository>();

        return services;
    }
}