using HotelMaintenance.Infrastructure.Interfaces;
using HotelMaintenance.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelMaintenance.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register external services
        services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }
}
