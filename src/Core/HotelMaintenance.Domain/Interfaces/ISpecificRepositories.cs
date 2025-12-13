using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Interfaces;

public interface IMaintenanceOrderRepository : IRepository<MaintenanceOrder>
{
    Task<string> GenerateOrderNumberAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetOrdersByHotelAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetOrdersByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetAssignedOrdersAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceOrder>> GetSLABreachedOrdersAsync(CancellationToken cancellationToken = default);
    Task<MaintenanceOrder?> GetOrderWithDetailsAsync(long orderId, CancellationToken cancellationToken = default);
}

public interface IHotelRepository : IRepository<Hotel>
{
    Task<Hotel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Hotel>> GetActiveHotelsAsync(CancellationToken cancellationToken = default);
}

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IEnumerable<Department>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<Department?> GetMaintenanceDepartmentAsync(int hotelId, CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAvailableTechniciansAsync(int departmentId, CancellationToken cancellationToken = default);
}

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<Item>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetByLocationIdAsync(int locationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Item?> GetByCodeAsync(string code, int hotelId, CancellationToken cancellationToken = default);
}

public interface ISparePartRepository : IRepository<SparePart>
{
    Task<IEnumerable<SparePart>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparePart>> GetLowStockPartsAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<SparePart?> GetByCodeAsync(string code, int hotelId, CancellationToken cancellationToken = default);
}

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetByTypeAsync(int hotelId, LocationType type, CancellationToken cancellationToken = default);
}

public interface IVendorRepository : IRepository<Vendor>
{
    Task<IEnumerable<Vendor>> GetActiveVendorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Vendor>> GetByTypeAsync(VendorType type, CancellationToken cancellationToken = default);
}

public interface ISLAConfigurationRepository : IRepository<SLAConfiguration>
{
    Task<SLAConfiguration?> GetByHotelAndPriorityAsync(int hotelId, OrderPriority priority, CancellationToken cancellationToken = default);
}
