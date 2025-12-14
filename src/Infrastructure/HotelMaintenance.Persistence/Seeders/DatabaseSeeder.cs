using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRolesAsync(context);
        await context.SaveChangesAsync();

        await SeedPermissionsAsync(context);
        await context.SaveChangesAsync();

        await SeedRolePermissionsAsync(context);
        await context.SaveChangesAsync();

        await SeedItemCategoriesAsync(context);
        await context.SaveChangesAsync();
    }

    // -------------------- ROLES --------------------
    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new[]
        {
            new Role { Name = "SystemAdmin", Description = "Full system access", IsSystemRole = true, IsActive = true },
            new Role { Name = "HotelManager", Description = "Hotel-level management", IsSystemRole = true, IsActive = true },
            new Role { Name = "DepartmentHead", Description = "Department management", IsSystemRole = true, IsActive = true },
            new Role { Name = "MaintenanceManager", Description = "Engineering manager", IsSystemRole = true, IsActive = true },
            new Role { Name = "Technician", Description = "Maintenance technician", IsSystemRole = true, IsActive = true },
            new Role { Name = "Staff", Description = "General staff", IsSystemRole = true, IsActive = true },
            new Role { Name = "Viewer", Description = "Read-only access", IsSystemRole = true, IsActive = true }
        };

        foreach (var role in roles)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == role.Name))
            {
                context.Roles.Add(role);
            }
        }
    }

    // -------------------- PERMISSIONS --------------------
    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        var permissions = new[]
        {
            new Permission { Name = "View Orders", Code = "Orders.View", Module = "Orders", IsActive = true },
            new Permission { Name = "Create Orders", Code = "Orders.Create", Module = "Orders", IsActive = true },
            new Permission { Name = "Update Orders", Code = "Orders.Update", Module = "Orders", IsActive = true },
            new Permission { Name = "Assign Orders", Code = "Orders.Assign", Module = "Orders", IsActive = true },
            new Permission { Name = "Complete Orders", Code = "Orders.Complete", Module = "Orders", IsActive = true },
            new Permission { Name = "Cancel Orders", Code = "Orders.Cancel", Module = "Orders", IsActive = true },
            new Permission { Name = "Verify Orders", Code = "Orders.Verify", Module = "Orders", IsActive = true },

            new Permission { Name = "View Users", Code = "Users.View", Module = "Users", IsActive = true },
            new Permission { Name = "Manage Users", Code = "Users.Manage", Module = "Users", IsActive = true },

            new Permission { Name = "View Departments", Code = "Departments.View", Module = "Departments", IsActive = true },
            new Permission { Name = "Manage Departments", Code = "Departments.Manage", Module = "Departments", IsActive = true },

            new Permission { Name = "View Items", Code = "Items.View", Module = "Items", IsActive = true },
            new Permission { Name = "Manage Items", Code = "Items.Manage", Module = "Items", IsActive = true },

            new Permission { Name = "View Reports", Code = "Reports.View", Module = "Reports", IsActive = true },
            new Permission { Name = "Export Reports", Code = "Reports.Export", Module = "Reports", IsActive = true },

            new Permission { Name = "View Dashboard", Code = "Dashboard.View", Module = "Dashboard", IsActive = true }
        };

        foreach (var perm in permissions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Code == perm.Code))
            {
                context.Permissions.Add(perm);
            }
        }
    }

    // -------------------- ROLE PERMISSIONS --------------------
    private static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
            return;

        var roles = await context.Roles.ToDictionaryAsync(r => r.Name, r => r.Id);
        var permissions = await context.Permissions.ToDictionaryAsync(p => p.Code, p => p.Id);

        var mappings = new Dictionary<string, string[]>
        {
            ["SystemAdmin"] = permissions.Keys.ToArray(),

            ["HotelManager"] = new[]
            {
                "Orders.View", "Orders.Create", "Orders.Assign",
                "Users.View", "Departments.View", "Items.View",
                "Reports.View", "Dashboard.View"
            },

            ["Technician"] = new[]
            {
                "Orders.View", "Orders.Update", "Orders.Complete"
            },

            ["Staff"] = new[]
            {
                "Orders.View", "Orders.Create"
            },

            ["Viewer"] = new[]
            {
                "Orders.View", "Reports.View", "Dashboard.View"
            }
        };

        foreach (var (roleName, permissionCodes) in mappings)
        {
            if (!roles.TryGetValue(roleName, out var roleId))
                continue;

            foreach (var code in permissionCodes)
            {
                if (!permissions.TryGetValue(code, out var permId))
                    continue;

                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId
                });
            }
        }
    }

    // -------------------- ITEM CATEGORIES --------------------
    private static async Task SeedItemCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.ItemCategories.AnyAsync())
            return;

        context.ItemCategories.AddRange(
            new ItemCategory { Code = "HVAC", Name = "HVAC Systems", IsActive = true },
            new ItemCategory { Code = "ELEC", Name = "Electrical", IsActive = true },
            new ItemCategory { Code = "PLUMB", Name = "Plumbing", IsActive = true },
            new ItemCategory { Code = "IT", Name = "IT Equipment", IsActive = true }
        );
    }
}
