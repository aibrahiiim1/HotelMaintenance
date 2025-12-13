using HotelMaintenance.Identity.Models;
using HotelMaintenance.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HotelMaintenance.Identity;

/// <summary>
/// Dependency injection configuration for Identity layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT settings
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettings);

        var jwtConfig = jwtSettings.Get<JwtSettings>();
        if (jwtConfig == null || string.IsNullOrEmpty(jwtConfig.SecretKey))
        {
            throw new InvalidOperationException("JWT settings are not configured properly");
        }

        // Add JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Add authorization
        services.AddAuthorization(options =>
        {
            // Order permissions
            options.AddPolicy("Orders.View", policy => policy.RequireClaim("permission", "Orders.View"));
            options.AddPolicy("Orders.Create", policy => policy.RequireClaim("permission", "Orders.Create"));
            options.AddPolicy("Orders.Update", policy => policy.RequireClaim("permission", "Orders.Update"));
            options.AddPolicy("Orders.Assign", policy => policy.RequireClaim("permission", "Orders.Assign"));
            options.AddPolicy("Orders.Complete", policy => policy.RequireClaim("permission", "Orders.Complete"));
            options.AddPolicy("Orders.Cancel", policy => policy.RequireClaim("permission", "Orders.Cancel"));
            options.AddPolicy("Orders.Delete", policy => policy.RequireClaim("permission", "Orders.Delete"));

            // User management
            options.AddPolicy("Users.Manage", policy => policy.RequireClaim("permission", "Users.Manage"));
            options.AddPolicy("Departments.Manage", policy => policy.RequireClaim("permission", "Departments.Manage"));
            options.AddPolicy("Items.Manage", policy => policy.RequireClaim("permission", "Items.Manage"));
            options.AddPolicy("SpareParts.Manage", policy => policy.RequireClaim("permission", "SpareParts.Manage"));

            // Reports
            options.AddPolicy("Reports.View", policy => policy.RequireClaim("permission", "Reports.View"));
            options.AddPolicy("Dashboard.View", policy => policy.RequireClaim("permission", "Dashboard.View"));

            // System admin
            options.AddPolicy("System.Admin", policy => policy.RequireClaim("permission", "System.Admin"));

            // Role-based policies
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("SystemAdmin"));
            options.AddPolicy("RequireManagerRole", policy => 
                policy.RequireRole("SystemAdmin", "HotelManager", "MaintenanceManager"));
        });

        // Register services
        services.AddScoped<JwtTokenService>();
        services.AddScoped<AuthenticationService>();

        return services;
    }
}
