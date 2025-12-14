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
        // Seed Hotels FIRST
        await SeedHotelsAsync(context);

        // Seed Departments (depends on Hotels)
        await SeedDepartmentsAsync(context);

        // Seed Roles
        await SeedRolesAsync(context);

        // Seed Permissions
        await SeedPermissionsAsync(context);

        // Seed Role-Permission mappings
        await SeedRolePermissionsAsync(context);

        // Seed Users (depends on Hotels and Departments)
        await SeedUsersAsync(context);

        // Seed Locations (depends on Hotels)
        await SeedLocationsAsync(context);

        // Seed Item Categories
        await SeedItemCategoriesAsync(context);

        // Seed SLA Configurations (depends on Hotels)
        await SeedSLAConfigurationsAsync(context);
        await SeedUserRolesAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedHotelsAsync(ApplicationDbContext context)
    {
        if (await context.Hotels.AnyAsync())
            return;

        var hotels = new List<Hotel>
        {
            new()
            {
                Code = "HTL001",
                Name = "Grand Plaza Hotel",
                Description = "5-star luxury hotel in downtown",
                Address = "123 Main Street",
                City = "Cairo",
                State = "Cairo",
                Country = "Egypt",
                PostalCode = "11511",
                Phone = "+20-2-12345678",
                Email = "info@grandplaza.com",
                TimeZone = "Africa/Cairo",
                TotalRooms = 250,
                StarRating = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                Code = "HTL002",
                Name = "Seaside Resort",
                Description = "Beachfront resort hotel",
                Address = "456 Beach Road",
                City = "Sharm El Sheikh",
                State = "South Sinai",
                Country = "Egypt",
                PostalCode = "46619",
                Phone = "+20-69-9876543",
                Email = "info@seasideresort.com",
                TimeZone = "Africa/Cairo",
                TotalRooms = 180,
                StarRating = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            }
        };

        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync(); // Save hotels first so they have IDs
    }

    private static async Task SeedDepartmentsAsync(ApplicationDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;

        // Get the hotels that were just seeded
        var grandPlaza = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL001");
        var seasideResort = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL002");

        if (grandPlaza == null || seasideResort == null)
            return; // Hotels not seeded yet

        var departments = new List<Department>
        {
            // Grand Plaza Hotel Departments
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "ENG",
                Name = "Engineering & Maintenance",
                Description = "Handles all maintenance and repair work",
                Type = DepartmentType.Engineering,
                IsMaintenanceProvider = true,
                CanCreateOrders = true,
                CanReceiveOrders = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "FO",
                Name = "Front Office",
                Description = "Guest services and reception",
                Type = DepartmentType.FrontOffice,
                CanCreateOrders = true,
                CanReceiveOrders = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "HK",
                Name = "Housekeeping",
                Description = "Room cleaning and maintenance",
                Type = DepartmentType.Housekeeping,
                CanCreateOrders = true,
                CanReceiveOrders = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "FB",
                Name = "Food & Beverage",
                Description = "Restaurant and kitchen operations",
                Type = DepartmentType.FoodAndBeverage,
                CanCreateOrders = true,
                CanReceiveOrders = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Seaside Resort Departments
            new()
            {
                HotelId = seasideResort.Id,
                Code = "ENG",
                Name = "Engineering & Maintenance",
                Description = "Handles all maintenance and repair work",
                Type = DepartmentType.Engineering,
                IsMaintenanceProvider = true,
                CanCreateOrders = true,
                CanReceiveOrders = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = seasideResort.Id,
                Code = "FO",
                Name = "Front Office",
                Description = "Guest services and reception",
                Type = DepartmentType.FrontOffice,
                CanCreateOrders = true,
                CanReceiveOrders = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            }
        };

        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync(); // Save departments so they have IDs
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        // Get the hotels and departments that were just seeded
        var grandPlaza = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL001");
        var seasideResort = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL002");

        var grandPlazaEng = await context.Departments.FirstOrDefaultAsync(d => d.HotelId == grandPlaza.Id && d.Code == "ENG");
        var grandPlazaFO = await context.Departments.FirstOrDefaultAsync(d => d.HotelId == grandPlaza.Id && d.Code == "FO");
        var seasideEng = await context.Departments.FirstOrDefaultAsync(d => d.HotelId == seasideResort.Id && d.Code == "ENG");

        if (grandPlaza == null || grandPlazaEng == null)
            return; // Dependencies not seeded yet

        // Hash the default password "Password123!" for all users
        var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

        var users = new List<User>
        {
            // System Admin
            new()
            {
                EmployeeId = "EMP001",
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@hotel.com",
                PasswordHash = defaultPasswordHash,
                Phone = "+20-1234567890",
                HotelId = grandPlaza.Id,
                DepartmentId = grandPlazaEng.Id,
                JobTitle = "System Administrator",
                IsAvailable = true,
                IsActive = true,
                EmailNotifications = true,
                SmsNotifications = false,
                PushNotifications = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Hotel 1 - Engineering Manager
            new()
            {
                EmployeeId = "EMP002",
                FirstName = "Ahmed",
                LastName = "Hassan",
                Email = "ahmed.hassan@grandplaza.com",
                PasswordHash = defaultPasswordHash,
                Phone = "+20-1011223344",
                HotelId = grandPlaza.Id,
                DepartmentId = grandPlazaEng.Id,
                JobTitle = "Chief Engineer",
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Hotel 1 - Technician
            new()
            {
                EmployeeId = "EMP003",
                FirstName = "Mohamed",
                LastName = "Ali",
                Email = "mohamed.ali@grandplaza.com",
                PasswordHash = defaultPasswordHash,
                Phone = "+20-1022334455",
                HotelId = grandPlaza.Id,
                DepartmentId = grandPlazaEng.Id,
                JobTitle = "Senior Technician",
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Hotel 1 - Front Office
            new()
            {
                EmployeeId = "EMP004",
                FirstName = "Sara",
                LastName = "Ibrahim",
                Email = "sara.ibrahim@grandplaza.com",
                PasswordHash = defaultPasswordHash,
                Phone = "+20-1033445566",
                HotelId = grandPlaza.Id,
                DepartmentId = grandPlazaFO.Id,
                JobTitle = "Front Office Manager",
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Hotel 2 - Engineering Manager
            new()
            {
                EmployeeId = "EMP005",
                FirstName = "Karim",
                LastName = "Mansour",
                Email = "karim.mansour@seasideresort.com",
                PasswordHash = defaultPasswordHash,
                Phone = "+20-1044556677",
                HotelId = seasideResort.Id,
                DepartmentId = seasideEng.Id,
                JobTitle = "Maintenance Manager",
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync(); // Save users so they have IDs
    }
    private static async Task SeedUserRolesAsync(ApplicationDbContext context)
    {
        if (await context.UserRoles.AnyAsync())
            return;

        // Get roles by name
        var roles = await context.Roles.ToDictionaryAsync(r => r.Name, r => r.Id);

        // Get users by email
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@hotel.com");
        var ahmed = await context.Users.FirstOrDefaultAsync(u => u.Email == "ahmed.hassan@grandplaza.com");
        var mohamed = await context.Users.FirstOrDefaultAsync(u => u.Email == "mohamed.ali@grandplaza.com");
        var sara = await context.Users.FirstOrDefaultAsync(u => u.Email == "sara.ibrahim@grandplaza.com");
        var karim = await context.Users.FirstOrDefaultAsync(u => u.Email == "karim.mansour@seasideresort.com");

        var userRoles = new List<UserRole>();

        // Admin - SystemAdmin role
        if (admin != null && roles.TryGetValue("SystemAdmin", out var systemAdminRoleId))
        {
            userRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = systemAdminRoleId,
                HotelId = admin.HotelId,
                DepartmentId = admin.DepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = 1
            });
        }

        // Ahmed - MaintenanceManager role
        if (ahmed != null && roles.TryGetValue("MaintenanceManager", out var maintManagerRoleId))
        {
            userRoles.Add(new UserRole
            {
                UserId = ahmed.Id,
                RoleId = maintManagerRoleId,
                HotelId = ahmed.HotelId,
                DepartmentId = ahmed.DepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = 1
            });
        }

        // Mohamed - Technician role
        if (mohamed != null && roles.TryGetValue("Technician", out var technicianRoleId))
        {
            userRoles.Add(new UserRole
            {
                UserId = mohamed.Id,
                RoleId = technicianRoleId,
                HotelId = mohamed.HotelId,
                DepartmentId = mohamed.DepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = 1
            });
        }

        // Sara - DepartmentHead role
        if (sara != null && roles.TryGetValue("DepartmentHead", out var deptHeadRoleId))
        {
            userRoles.Add(new UserRole
            {
                UserId = sara.Id,
                RoleId = deptHeadRoleId,
                HotelId = sara.HotelId,
                DepartmentId = sara.DepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = 1
            });
        }

        // Karim - MaintenanceManager role
        if (karim != null && roles.TryGetValue("MaintenanceManager", out var maintManagerRoleId2))
        {
            userRoles.Add(new UserRole
            {
                UserId = karim.Id,
                RoleId = maintManagerRoleId2,
                HotelId = karim.HotelId,
                DepartmentId = karim.DepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = 1
            });
        }

        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
    }
    private static async Task SeedLocationsAsync(ApplicationDbContext context)
    {
        if (await context.Locations.AnyAsync())
            return;

        // Get the hotels
        var grandPlaza = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL001");
        var seasideResort = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL002");

        if (grandPlaza == null || seasideResort == null)
            return;

        var locations = new List<Location>
        {
            // Grand Plaza Hotel
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "BLDG-MAIN",
                Name = "Main Building",
                Description = "Primary guest tower",
                Type = LocationType.Spa,
                Building = "Main",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "LOBBY",
                Name = "Main Lobby",
                Description = "Hotel main entrance and reception",
                Type = LocationType.PublicArea,
                Building = "Main",
                Floor = "G",
                Zone = "Lobby",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "RM-101",
                Name = "Room 101",
                Description = "Deluxe King Room",
                Type = LocationType.GuestRoom,
                Building = "Main",
                Floor = "1",
                RoomNumber = "101",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Code = "BOILER",
                Name = "Boiler Room",
                Description = "Mechanical equipment room",
                Type = LocationType.Office,
                Building = "Main",
                Floor = "B1",
                Zone = "Basement",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            // Seaside Resort
            new()
            {
                HotelId = seasideResort.Id,
                Code = "BLDG-BEACH",
                Name = "Beach Tower",
                Description = "Beachfront accommodation",
                Type = LocationType.Spa,
                Building = "Beach",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            },
            new()
            {
                HotelId = seasideResort.Id,
                Code = "POOL",
                Name = "Main Pool Area",
                Description = "Resort swimming pool and deck",
                Type = LocationType.Pool,
                Building = "Beach",
                Zone = "Pool",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                LastModifiedAt = DateTime.UtcNow
            }
        };

        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new() { Name = "SystemAdmin", Description = "Full system access", IsSystemRole = true, IsActive = true },
            new() { Name = "HotelManager", Description = "Hotel-level management", IsSystemRole = true, IsActive = true },
            new() { Name = "DepartmentHead", Description = "Department management", IsSystemRole = true, IsActive = true },
            new() { Name = "MaintenanceManager", Description = "Engineering/Maintenance manager", IsSystemRole = true, IsActive = true },
            new() { Name = "Technician", Description = "Maintenance technician", IsSystemRole = true, IsActive = true },
            new() { Name = "Staff", Description = "General staff (can create orders)", IsSystemRole = true, IsActive = true },
            new() { Name = "Viewer", Description = "Read-only access", IsSystemRole = true, IsActive = true }
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
            new() { Name = "View Orders", Code = "Orders.View", Module = "Orders", IsActive = true },
            new() { Name = "Create Orders", Code = "Orders.Create", Module = "Orders", IsActive = true },
            new() { Name = "Update Orders", Code = "Orders.Update", Module = "Orders", IsActive = true },
            new() { Name = "Assign Orders", Code = "Orders.Assign", Module = "Orders", IsActive = true },
            new() { Name = "Complete Orders", Code = "Orders.Complete", Module = "Orders", IsActive = true },
            new() { Name = "Cancel Orders", Code = "Orders.Cancel", Module = "Orders", IsActive = true },
            new() { Name = "Delete Orders", Code = "Orders.Delete", Module = "Orders", IsActive = true },
            new() { Name = "Verify Orders", Code = "Orders.Verify", Module = "Orders", IsActive = true },

            // User Management
            new() { Name = "View Users", Code = "Users.View", Module = "Users", IsActive = true },
            new() { Name = "Manage Users", Code = "Users.Manage", Module = "Users", IsActive = true },

            // Department Management
            new() { Name = "View Departments", Code = "Departments.View", Module = "Departments", IsActive = true },
            new() { Name = "Manage Departments", Code = "Departments.Manage", Module = "Departments", IsActive = true },

            // Item Management
            new() { Name = "View Items", Code = "Items.View", Module = "Items", IsActive = true },
            new() { Name = "Manage Items", Code = "Items.Manage", Module = "Items", IsActive = true },

            // Spare Parts
            new() { Name = "View Spare Parts", Code = "SpareParts.View", Module = "SpareParts", IsActive = true },
            new() { Name = "Manage Spare Parts", Code = "SpareParts.Manage", Module = "SpareParts", IsActive = true },

            // Reports
            new() { Name = "View Reports", Code = "Reports.View", Module = "Reports", IsActive = true },
            new() { Name = "Export Reports", Code = "Reports.Export", Module = "Reports", IsActive = true },

            // Dashboard
            new() { Name = "View Dashboard", Code = "Dashboard.View", Module = "Dashboard", IsActive = true },

            // System
            new() { Name = "System Administration", Code = "System.Admin", Module = "System", IsActive = true }
        };

        await context.Permissions.AddRangeAsync(permissions);
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
            return;

        // Get roles and permissions by their names/codes (not IDs)
        var roles = await context.Roles.ToDictionaryAsync(r => r.Name, r => r.Id);
        var permissions = await context.Permissions.ToDictionaryAsync(p => p.Code, p => p.Id);

        var rolePermissions = new List<RolePermission>();

        // SystemAdmin - All permissions
        if (roles.TryGetValue("SystemAdmin", out var systemAdminId))
        {
            foreach (var perm in permissions.Values)
            {
                rolePermissions.Add(new RolePermission { RoleId = systemAdminId, PermissionId = perm });
            }
        }

        // HotelManager
        if (roles.TryGetValue("HotelManager", out var hotelManagerId))
        {
            var hotelManagerPerms = new[] { "Orders.View", "Orders.Create", "Orders.Update", "Orders.Assign", "Orders.Complete", "Orders.Cancel", "Orders.Verify",
                "Users.View", "Users.Manage", "Departments.View", "Departments.Manage", "Items.View", "Items.Manage",
                "SpareParts.View", "SpareParts.Manage", "Reports.View", "Reports.Export", "Dashboard.View" };
            foreach (var code in hotelManagerPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = hotelManagerId, PermissionId = permId });
            }
        }

        // DepartmentHead
        if (roles.TryGetValue("DepartmentHead", out var deptHeadId))
        {
            var deptHeadPerms = new[] { "Orders.View", "Orders.Create", "Orders.Update", "Orders.Assign", "Orders.Complete", "Orders.Cancel", "Orders.Verify",
                "Users.View", "Items.View", "SpareParts.View", "Reports.View", "Dashboard.View" };
            foreach (var code in deptHeadPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = deptHeadId, PermissionId = permId });
            }
        }

        // MaintenanceManager
        if (roles.TryGetValue("MaintenanceManager", out var maintManagerId))
        {
            var maintManagerPerms = new[] { "Orders.View", "Orders.Create", "Orders.Update", "Orders.Assign", "Orders.Complete", "Orders.Cancel", "Orders.Verify",
                "Items.View", "Items.Manage", "SpareParts.View", "SpareParts.Manage", "Reports.View", "Reports.Export", "Dashboard.View" };
            foreach (var code in maintManagerPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = maintManagerId, PermissionId = permId });
            }
        }

        // Technician
        if (roles.TryGetValue("Technician", out var technicianId))
        {
            var technicianPerms = new[] { "Orders.View", "Orders.Update", "Orders.Complete", "Items.View", "SpareParts.View" };
            foreach (var code in technicianPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = technicianId, PermissionId = permId });
            }
        }

        // Staff
        if (roles.TryGetValue("Staff", out var staffId))
        {
            var staffPerms = new[] { "Orders.View", "Orders.Create", "Orders.Verify", "Items.View", "SpareParts.View" };
            foreach (var code in staffPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = staffId, PermissionId = permId });
            }
        }

        // Viewer
        if (roles.TryGetValue("Viewer", out var viewerId))
        {
            var viewerPerms = new[] { "Orders.View", "Users.View", "Departments.View", "Items.View", "SpareParts.View", "Reports.View", "Dashboard.View" };
            foreach (var code in viewerPerms)
            {
                if (permissions.TryGetValue(code, out var permId))
                    rolePermissions.Add(new RolePermission { RoleId = viewerId, PermissionId = permId });
            }
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
    }

    private static async Task SeedItemCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.ItemCategories.AnyAsync())
            return;

        var categories = new List<ItemCategory>
        {
            new() { Code = "HVAC", Name = "HVAC Systems", Description = "Heating, Ventilation, and Air Conditioning", Icon = "ac_unit", SortOrder = 1, IsActive = true },
            new() { Code = "ELEC", Name = "Electrical", Description = "Electrical systems and equipment", Icon = "electrical_services", SortOrder = 2, IsActive = true },
            new() { Code = "PLUMB", Name = "Plumbing", Description = "Plumbing systems and fixtures", Icon = "plumbing", SortOrder = 3, IsActive = true },
            new() { Code = "KITCH", Name = "Kitchen Equipment", Description = "Commercial kitchen appliances", Icon = "kitchen", SortOrder = 4, IsActive = true },
            new() { Code = "LAUND", Name = "Laundry Equipment", Description = "Washers, dryers, and related", Icon = "local_laundry_service", SortOrder = 5, IsActive = true },
            new() { Code = "ELEV", Name = "Elevators", Description = "Elevator systems", Icon = "elevator", SortOrder = 6, IsActive = true },
            new() { Code = "FIRE", Name = "Fire Safety", Description = "Fire suppression and safety", Icon = "local_fire_department", SortOrder = 7, IsActive = true },
            new() { Code = "SECUR", Name = "Security", Description = "Access control and CCTV", Icon = "security", SortOrder = 8, IsActive = true },
            new() { Code = "FURN", Name = "Furniture", Description = "Hotel furniture", Icon = "chair", SortOrder = 9, IsActive = true },
            new() { Code = "IT", Name = "IT Equipment", Description = "Computers, networks, telecom", Icon = "computer", SortOrder = 10, IsActive = true }
        };

        await context.ItemCategories.AddRangeAsync(categories);
    }

    private static async Task SeedSLAConfigurationsAsync(ApplicationDbContext context)
    {
        if (await context.SLAConfigurations.AnyAsync())
            return;

        // Get the Grand Plaza hotel
        var grandPlaza = await context.Hotels.FirstOrDefaultAsync(h => h.Code == "HTL001");
        if (grandPlaza == null)
            return; // Hotel not seeded yet

        var slaTemplates = new List<SLAConfiguration>
        {
            new()
            {
                HotelId = grandPlaza.Id,
                Priority = OrderPriority.Critical,
                ResponseTimeMinutes = 15,
                ResolutionTimeMinutes = 120,
                EnableEscalation = true,
                EscalationThresholdMinutes = 30,
                IsActive = true
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Priority = OrderPriority.High,
                ResponseTimeMinutes = 60,
                ResolutionTimeMinutes = 480,
                EnableEscalation = true,
                EscalationThresholdMinutes = 60,
                IsActive = true
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Priority = OrderPriority.Medium,
                ResponseTimeMinutes = 240,
                ResolutionTimeMinutes = 1440,
                EnableEscalation = false,
                IsActive = true
            },
            new()
            {
                HotelId = grandPlaza.Id,
                Priority = OrderPriority.Low,
                ResponseTimeMinutes = 1440,
                ResolutionTimeMinutes = 10080,
                EnableEscalation = false,
                IsActive = true
            }
        };

        await context.SLAConfigurations.AddRangeAsync(slaTemplates);
    }
}