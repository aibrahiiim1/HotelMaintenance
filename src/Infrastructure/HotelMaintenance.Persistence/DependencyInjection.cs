using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Persistence.Context;
using HotelMaintenance.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelMaintenance.Persistence;

/// <summary>
/// Dependency injection configuration for Persistence layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            
            // Enable sensitive data logging in development
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register specific repositories (if needed separately)
        services.AddScoped<IMaintenanceOrderRepository, MaintenanceOrderRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ISparePartRepository, SparePartRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<ISLAConfigurationRepository, SLAConfigurationRepository>();

        return services;
    }
}
