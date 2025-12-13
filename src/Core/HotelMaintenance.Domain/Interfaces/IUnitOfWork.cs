using HotelMaintenance.Domain.Entities;

namespace HotelMaintenance.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern for managing transactions across multiple repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IMaintenanceOrderRepository MaintenanceOrders { get; }
    IHotelRepository Hotels { get; }
    IDepartmentRepository Departments { get; }
    IUserRepository Users { get; }
    IItemRepository Items { get; }
    ISparePartRepository SpareParts { get; }
    ILocationRepository Locations { get; }
    IVendorRepository Vendors { get; }
    ISLAConfigurationRepository SLAConfigurations { get; }
    
    IRepository<OrderStatusHistory> OrderStatusHistory { get; }
    IRepository<OrderAssignmentHistory> OrderAssignmentHistory { get; }
    IRepository<OrderComment> OrderComments { get; }
    IRepository<OrderAttachment> OrderAttachments { get; }
    IRepository<OrderChecklistItem> OrderChecklistItems { get; }
    IRepository<OrderSparePartUsage> OrderSparePartUsage { get; }
    IRepository<SparePartTransaction> SparePartTransactions { get; }
    IRepository<ItemCategory> ItemCategories { get; }
    IRepository<ItemClass> ItemClasses { get; }
    IRepository<ItemFamily> ItemFamilies { get; }
    IRepository<Role> Roles { get; }
    IRepository<Permission> Permissions { get; }
    IRepository<UserRole> UserRoles { get; }
    IRepository<PreventiveMaintenanceSchedule> PMSchedules { get; }
    IRepository<ChecklistTemplate> ChecklistTemplates { get; }
    IRepository<NotificationLog> NotificationLogs { get; }
    IRepository<AuditLog> AuditLogs { get; }
    
    // Transaction methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
