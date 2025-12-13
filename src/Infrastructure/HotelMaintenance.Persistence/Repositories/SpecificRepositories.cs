using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;
using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelMaintenance.Persistence.Repositories;

public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Hotel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Departments)
            .FirstOrDefaultAsync(h => h.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Hotel>> GetActiveHotelsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.IsActive)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);
    }
}

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetByHotelIdAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Manager)
            .Where(d => d.HotelId == hotelId && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetMaintenanceDepartmentAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.HotelId == hotelId && 
                                     d.IsMaintenanceProvider && 
                                     d.IsActive, 
                                cancellationToken);
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Hotel)
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Hotel)
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAvailableTechniciansAsync(
        int departmentId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.DepartmentId == departmentId && 
                       u.IsAvailable && 
                       u.IsActive)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }
}

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Item>> GetByHotelIdAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Category)
            .Include(i => i.Class)
            .Include(i => i.Family)
            .Include(i => i.Location)
            .Where(i => i.HotelId == hotelId && i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetByLocationIdAsync(
        int locationId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Category)
            .Include(i => i.Class)
            .Where(i => i.LocationId == locationId && i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetByCategoryIdAsync(
        int categoryId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Location)
            .Where(i => i.CategoryId == categoryId && i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetByCodeAsync(
        string code, 
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Category)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(i => i.Code == code && i.HotelId == hotelId, cancellationToken);
    }
}

public class SparePartRepository : Repository<SparePart>, ISparePartRepository
{
    public SparePartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SparePart>> GetByHotelIdAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Item)
            .Include(s => s.StorageDepartment)
            .Where(s => s.HotelId == hotelId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparePart>> GetLowStockPartsAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Item)
            .Include(s => s.StorageDepartment)
            .Where(s => s.HotelId == hotelId && 
                       s.IsActive && 
                       s.QuantityOnHand <= s.MinimumQuantity)
            .OrderBy(s => s.QuantityOnHand)
            .ToListAsync(cancellationToken);
    }

    public async Task<SparePart?> GetByCodeAsync(
        string code, 
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Item)
            .Include(s => s.StorageDepartment)
            .FirstOrDefaultAsync(s => s.Code == code && s.HotelId == hotelId, cancellationToken);
    }
}

public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Location>> GetByHotelIdAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.ParentLocation)
            .Where(l => l.HotelId == hotelId && l.IsActive)
            .OrderBy(l => l.Building)
            .ThenBy(l => l.Floor)
            .ThenBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByTypeAsync(
        int hotelId, 
        LocationType type, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.HotelId == hotelId && l.Type == type && l.IsActive)
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }
}

public class VendorRepository : Repository<Vendor>, IVendorRepository
{
    public VendorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Vendor>> GetActiveVendorsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Vendor>> GetByTypeAsync(
        VendorType type, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.Type == type && v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }
}

public class SLAConfigurationRepository : Repository<SLAConfiguration>, ISLAConfigurationRepository
{
    public SLAConfigurationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SLAConfiguration?> GetByHotelAndPriorityAsync(
        int hotelId, 
        OrderPriority priority, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.HotelId == hotelId && 
                                     s.Priority == priority && 
                                     s.IsActive, 
                                cancellationToken);
    }
}
