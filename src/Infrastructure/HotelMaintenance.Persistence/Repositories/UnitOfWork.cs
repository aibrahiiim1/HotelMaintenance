using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelMaintenance.Persistence.Repositories;

/// <summary>
/// Unit of Work implementation for managing transactions across multiple repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy-loaded repositories
    private IMaintenanceOrderRepository? _maintenanceOrders;
    private IHotelRepository? _hotels;
    private IDepartmentRepository? _departments;
    private IUserRepository? _users;
    private IItemRepository? _items;
    private ISparePartRepository? _spareParts;
    private ILocationRepository? _locations;
    private IVendorRepository? _vendors;
    private ISLAConfigurationRepository? _slaConfigurations;
    
    private IRepository<OrderStatusHistory>? _orderStatusHistory;
    private IRepository<OrderAssignmentHistory>? _orderAssignmentHistory;
    private IRepository<OrderComment>? _orderComments;
    private IRepository<OrderAttachment>? _orderAttachments;
    private IRepository<OrderChecklistItem>? _orderChecklistItems;
    private IRepository<OrderSparePartUsage>? _orderSparePartUsage;
    private IRepository<SparePartTransaction>? _sparePartTransactions;
    private IRepository<ItemCategory>? _itemCategories;
    private IRepository<ItemClass>? _itemClasses;
    private IRepository<ItemFamily>? _itemFamilies;
    private IRepository<Role>? _roles;
    private IRepository<Permission>? _permissions;
    private IRepository<UserRole>? _userRoles;
    private IRepository<PreventiveMaintenanceSchedule>? _pmSchedules;
    private IRepository<ChecklistTemplate>? _checklistTemplates;
    private IRepository<NotificationLog>? _notificationLogs;
    private IRepository<AuditLog>? _auditLogs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lazy-load repository instances
    public IMaintenanceOrderRepository MaintenanceOrders => 
        _maintenanceOrders ??= new MaintenanceOrderRepository(_context);

    public IHotelRepository Hotels => 
        _hotels ??= new HotelRepository(_context);

    public IDepartmentRepository Departments => 
        _departments ??= new DepartmentRepository(_context);

    public IUserRepository Users => 
        _users ??= new UserRepository(_context);

    public IItemRepository Items => 
        _items ??= new ItemRepository(_context);

    public ISparePartRepository SpareParts => 
        _spareParts ??= new SparePartRepository(_context);

    public ILocationRepository Locations => 
        _locations ??= new LocationRepository(_context);

    public IVendorRepository Vendors => 
        _vendors ??= new VendorRepository(_context);

    public ISLAConfigurationRepository SLAConfigurations => 
        _slaConfigurations ??= new SLAConfigurationRepository(_context);

    public IRepository<OrderStatusHistory> OrderStatusHistory => 
        _orderStatusHistory ??= new Repository<OrderStatusHistory>(_context);

    public IRepository<OrderAssignmentHistory> OrderAssignmentHistory => 
        _orderAssignmentHistory ??= new Repository<OrderAssignmentHistory>(_context);

    public IRepository<OrderComment> OrderComments => 
        _orderComments ??= new Repository<OrderComment>(_context);

    public IRepository<OrderAttachment> OrderAttachments => 
        _orderAttachments ??= new Repository<OrderAttachment>(_context);

    public IRepository<OrderChecklistItem> OrderChecklistItems => 
        _orderChecklistItems ??= new Repository<OrderChecklistItem>(_context);

    public IRepository<OrderSparePartUsage> OrderSparePartUsage => 
        _orderSparePartUsage ??= new Repository<OrderSparePartUsage>(_context);

    public IRepository<SparePartTransaction> SparePartTransactions => 
        _sparePartTransactions ??= new Repository<SparePartTransaction>(_context);

    public IRepository<ItemCategory> ItemCategories => 
        _itemCategories ??= new Repository<ItemCategory>(_context);

    public IRepository<ItemClass> ItemClasses => 
        _itemClasses ??= new Repository<ItemClass>(_context);

    public IRepository<ItemFamily> ItemFamilies => 
        _itemFamilies ??= new Repository<ItemFamily>(_context);

    public IRepository<Role> Roles => 
        _roles ??= new Repository<Role>(_context);

    public IRepository<Permission> Permissions => 
        _permissions ??= new Repository<Permission>(_context);

    public IRepository<UserRole> UserRoles => 
        _userRoles ??= new Repository<UserRole>(_context);

    public IRepository<PreventiveMaintenanceSchedule> PMSchedules => 
        _pmSchedules ??= new Repository<PreventiveMaintenanceSchedule>(_context);

    public IRepository<ChecklistTemplate> ChecklistTemplates => 
        _checklistTemplates ??= new Repository<ChecklistTemplate>(_context);

    public IRepository<NotificationLog> NotificationLogs => 
        _notificationLogs ??= new Repository<NotificationLog>(_context);

    public IRepository<AuditLog> AuditLogs => 
        _auditLogs ??= new Repository<AuditLog>(_context);

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
