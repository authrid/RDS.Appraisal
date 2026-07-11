using AppraisalSystem.Application.Interfaces;
using AppraisalSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppraisalSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAppraisalService, AppraisalService>();
        return services;
    }
}