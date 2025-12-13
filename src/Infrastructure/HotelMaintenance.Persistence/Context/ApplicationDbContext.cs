using HotelMaintenance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HotelMaintenance.Persistence.Context;

/// <summary>
/// Main database context for the Hotel Maintenance system
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Core Entities
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Location & Items
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemClass> ItemClasses => Set<ItemClass>();
    public DbSet<ItemFamily> ItemFamilies => Set<ItemFamily>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemAttachment> ItemAttachments => Set<ItemAttachment>();

    // Maintenance Orders
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<OrderAssignmentHistory> OrderAssignmentHistories => Set<OrderAssignmentHistory>();
    public DbSet<OrderComment> OrderComments => Set<OrderComment>();
    public DbSet<OrderAttachment> OrderAttachments => Set<OrderAttachment>();
    public DbSet<OrderChecklistItem> OrderChecklistItems => Set<OrderChecklistItem>();

    // Spare Parts
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<OrderSparePartUsage> OrderSparePartUsages => Set<OrderSparePartUsage>();
    public DbSet<SparePartTransaction> SparePartTransactions => Set<SparePartTransaction>();

    // Vendors
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorContract> VendorContracts => Set<VendorContract>();

    // Preventive Maintenance
    public DbSet<PreventiveMaintenanceSchedule> PreventiveMaintenanceSchedules => Set<PreventiveMaintenanceSchedule>();
    public DbSet<PMScheduleHistory> PMScheduleHistories => Set<PMScheduleHistory>();
    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItems => Set<ChecklistTemplateItem>();

    // System
    public DbSet<SLAConfiguration> SLAConfigurations => Set<SLAConfiguration>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Set default schema
        modelBuilder.HasDefaultSchema("dbo");

        // Global query filters for soft delete
        modelBuilder.Entity<MaintenanceOrder>().HasQueryFilter(o => !o.IsCancelled);
    }

    /// <summary>
    /// Override SaveChanges to automatically set audit fields
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically set audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically update CreatedAt, LastModifiedAt fields
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.AuditableEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            entity.LastModifiedAt = DateTime.UtcNow;
        }
    }
}
