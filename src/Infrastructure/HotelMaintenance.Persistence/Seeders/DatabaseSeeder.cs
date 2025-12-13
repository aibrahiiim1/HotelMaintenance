using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;
using HotelMaintenance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelMaintenance.Persistence.Seeders;

/// <summary>
/// Seeds the database with initial data
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Roles
        await SeedRolesAsync(context);
        
        // Seed Permissions
        await SeedPermissionsAsync(context);
        
        // Seed Role-Permission mappings
        await SeedRolePermissionsAsync(context);
        
        // Seed Item Categories
        await SeedItemCategoriesAsync(context);
        
        // Seed SLA Configurations (sample)
        await SeedSLAConfigurationsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "SystemAdmin", Description = "Full system access", IsSystemRole = true, IsActive = true },
            new() { Id = 2, Name = "HotelManager", Description = "Hotel-level management", IsSystemRole = true, IsActive = true },
            new() { Id = 3, Name = "DepartmentHead", Description = "Department management", IsSystemRole = true, IsActive = true },
            new() { Id = 4, Name = "MaintenanceManager", Description = "Engineering/Maintenance manager", IsSystemRole = true, IsActive = true },
            new() { Id = 5, Name = "Technician", Description = "Maintenance technician", IsSystemRole = true, IsActive = true },
            new() { Id = 6, Name = "Staff", Description = "General staff (can create orders)", IsSystemRole = true, IsActive = true },
            new() { Id = 7, Name = "Viewer", Description = "Read-only access", IsSystemRole = true, IsActive = true }
        };

        await context.Roles.AddRangeAsync(roles);
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync())
            return;

        var permissions = new List<Permission>
        {
            // Order Permissions
            new() { Id = 1, Name = "View Orders", Code = "Orders.View", Module = "Orders", IsActive = true },
            new() { Id = 2, Name = "Create Orders", Code = "Orders.Create", Module = "Orders", IsActive = true },
            new() { Id = 3, Name = "Update Orders", Code = "Orders.Update", Module = "Orders", IsActive = true },
            new() { Id = 4, Name = "Assign Orders", Code = "Orders.Assign", Module = "Orders", IsActive = true },
            new() { Id = 5, Name = "Complete Orders", Code = "Orders.Complete", Module = "Orders", IsActive = true },
            new() { Id = 6, Name = "Cancel Orders", Code = "Orders.Cancel", Module = "Orders", IsActive = true },
            new() { Id = 7, Name = "Delete Orders", Code = "Orders.Delete", Module = "Orders", IsActive = true },
            new() { Id = 8, Name = "Verify Orders", Code = "Orders.Verify", Module = "Orders", IsActive = true },

            // User Management
            new() { Id = 9, Name = "View Users", Code = "Users.View", Module = "Users", IsActive = true },
            new() { Id = 10, Name = "Manage Users", Code = "Users.Manage", Module = "Users", IsActive = true },

            // Department Management
            new() { Id = 11, Name = "View Departments", Code = "Departments.View", Module = "Departments", IsActive = true },
            new() { Id = 12, Name = "Manage Departments", Code = "Departments.Manage", Module = "Departments", IsActive = true },

            // Item Management
            new() { Id = 13, Name = "View Items", Code = "Items.View", Module = "Items", IsActive = true },
            new() { Id = 14, Name = "Manage Items", Code = "Items.Manage", Module = "Items", IsActive = true },

            // Spare Parts
            new() { Id = 15, Name = "View Spare Parts", Code = "SpareParts.View", Module = "SpareParts", IsActive = true },
            new() { Id = 16, Name = "Manage Spare Parts", Code = "SpareParts.Manage", Module = "SpareParts", IsActive = true },

            // Reports
            new() { Id = 17, Name = "View Reports", Code = "Reports.View", Module = "Reports", IsActive = true },
            new() { Id = 18, Name = "Export Reports", Code = "Reports.Export", Module = "Reports", IsActive = true },

            // Dashboard
            new() { Id = 19, Name = "View Dashboard", Code = "Dashboard.View", Module = "Dashboard", IsActive = true },

            // System
            new() { Id = 20, Name = "System Administration", Code = "System.Admin", Module = "System", IsActive = true }
        };

        await context.Permissions.AddRangeAsync(permissions);
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
            return;

        var rolePermissions = new List<RolePermission>();
        int id = 1;

        // SystemAdmin - All permissions
        for (int permId = 1; permId <= 20; permId++)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 1, PermissionId = permId });
        }

        // HotelManager
        var hotelManagerPerms = new[] { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        foreach (var permId in hotelManagerPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 2, PermissionId = permId });
        }

        // DepartmentHead
        var deptHeadPerms = new[] { 1, 2, 3, 4, 5, 6, 8, 9, 13, 15, 17, 19 };
        foreach (var permId in deptHeadPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 3, PermissionId = permId });
        }

        // MaintenanceManager
        var maintManagerPerms = new[] { 1, 2, 3, 4, 5, 6, 8, 13, 14, 15, 16, 17, 18, 19 };
        foreach (var permId in maintManagerPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 4, PermissionId = permId });
        }

        // Technician
        var technicianPerms = new[] { 1, 3, 5, 13, 15 };
        foreach (var permId in technicianPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 5, PermissionId = permId });
        }

        // Staff
        var staffPerms = new[] { 1, 2, 8, 13, 15 };
        foreach (var permId in staffPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 6, PermissionId = permId });
        }

        // Viewer
        var viewerPerms = new[] { 1, 9, 11, 13, 15, 17, 19 };
        foreach (var permId in viewerPerms)
        {
            rolePermissions.Add(new RolePermission { Id = id++, RoleId = 7, PermissionId = permId });
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
    }

    private static async Task SeedItemCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.ItemCategories.AnyAsync())
            return;

        var categories = new List<ItemCategory>
        {
            new() { Id = 1, Code = "HVAC", Name = "HVAC Systems", Description = "Heating, Ventilation, and Air Conditioning", Icon = "ac_unit", SortOrder = 1, IsActive = true },
            new() { Id = 2, Code = "ELEC", Name = "Electrical", Description = "Electrical systems and equipment", Icon = "electrical_services", SortOrder = 2, IsActive = true },
            new() { Id = 3, Code = "PLUMB", Name = "Plumbing", Description = "Plumbing systems and fixtures", Icon = "plumbing", SortOrder = 3, IsActive = true },
            new() { Id = 4, Code = "KITCH", Name = "Kitchen Equipment", Description = "Commercial kitchen appliances", Icon = "kitchen", SortOrder = 4, IsActive = true },
            new() { Id = 5, Code = "LAUND", Name = "Laundry Equipment", Description = "Washers, dryers, and related", Icon = "local_laundry_service", SortOrder = 5, IsActive = true },
            new() { Id = 6, Code = "ELEV", Name = "Elevators", Description = "Elevator systems", Icon = "elevator", SortOrder = 6, IsActive = true },
            new() { Id = 7, Code = "FIRE", Name = "Fire Safety", Description = "Fire suppression and safety", Icon = "local_fire_department", SortOrder = 7, IsActive = true },
            new() { Id = 8, Code = "SECUR", Name = "Security", Description = "Access control and CCTV", Icon = "security", SortOrder = 8, IsActive = true },
            new() { Id = 9, Code = "FURN", Name = "Furniture", Description = "Hotel furniture", Icon = "chair", SortOrder = 9, IsActive = true },
            new() { Id = 10, Code = "IT", Name = "IT Equipment", Description = "Computers, networks, telecom", Icon = "computer", SortOrder = 10, IsActive = true }
        };

        await context.ItemCategories.AddRangeAsync(categories);
    }

    private static async Task SeedSLAConfigurationsAsync(ApplicationDbContext context)
    {
        // This is sample data - actual SLA configs would be created per hotel
        // This is just to show the structure
        if (await context.SLAConfigurations.AnyAsync())
            return;

        // Note: In production, these would be created for each hotel
        // This is just a template/example
        var slaTemplates = new List<SLAConfiguration>
        {
            new() 
            { 
                Id = 1, 
                HotelId = 1, // Will need to be updated to actual hotel IDs
                Priority = OrderPriority.Critical,
                ResponseTimeMinutes = 15,
                ResolutionTimeMinutes = 120,
                EnableEscalation = true,
                EscalationThresholdMinutes = 30,
                IsActive = true
            },
            new() 
            { 
                Id = 2, 
                HotelId = 1,
                Priority = OrderPriority.High,
                ResponseTimeMinutes = 60,
                ResolutionTimeMinutes = 480,
                EnableEscalation = true,
                EscalationThresholdMinutes = 60,
                IsActive = true
            },
            new() 
            { 
                Id = 3, 
                HotelId = 1,
                Priority = OrderPriority.Medium,
                ResponseTimeMinutes = 240,
                ResolutionTimeMinutes = 1440,
                EnableEscalation = false,
                IsActive = true
            },
            new() 
            { 
                Id = 4, 
                HotelId = 1,
                Priority = OrderPriority.Low,
                ResponseTimeMinutes = 1440,
                ResolutionTimeMinutes = 10080,
                EnableEscalation = false,
                IsActive = true
            }
        };

        // Don't add if no hotels exist yet
        // await context.SLAConfigurations.AddRangeAsync(slaTemplates);
    }
}
