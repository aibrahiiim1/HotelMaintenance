using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;
using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelMaintenance.Persistence.Repositories;

public class MaintenanceOrderRepository : Repository<MaintenanceOrder>, IMaintenanceOrderRepository
{
    public MaintenanceOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<string> GenerateOrderNumberAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await _context.Hotels.FindAsync(new object[] { hotelId }, cancellationToken);
        if (hotel == null)
            throw new Exception($"Hotel with ID {hotelId} not found");

        var year = DateTime.UtcNow.Year;
        var prefix = $"MO-{hotel.Code}-{year}";

        var lastOrder = await _dbSet
            .Where(o => o.HotelId == hotelId && o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (lastOrder != null)
        {
            var lastNumberPart = lastOrder.OrderNumber.Split('-').Last();
            if (int.TryParse(lastNumberPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}-{nextNumber:D5}";
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOrdersByHotelAsync(
        int hotelId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.RequestingDepartment)
            .Include(o => o.AssignedDepartment)
            .Include(o => o.Location)
            .Include(o => o.Item)
            .Include(o => o.AssignedToUser)
            .Include(o => o.CreatedByUser)
            .Where(o => o.HotelId == hotelId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOrdersByDepartmentAsync(
        int departmentId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.RequestingDepartment)
            .Include(o => o.AssignedDepartment)
            .Include(o => o.Location)
            .Include(o => o.AssignedToUser)
            .Where(o => o.RequestingDepartmentId == departmentId || 
                       o.AssignedDepartmentId == departmentId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOrdersByStatusAsync(
        OrderStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.AssignedToUser)
            .Where(o => o.CurrentStatus == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetAssignedOrdersAsync(
        int userId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.RequestingDepartment)
            .Include(o => o.Location)
            .Include(o => o.Item)
            .Where(o => o.AssignedToUserId == userId && 
                       o.CurrentStatus != OrderStatus.Completed &&
                       o.CurrentStatus != OrderStatus.Closed &&
                       o.CurrentStatus != OrderStatus.Cancelled)
            .OrderBy(o => o.Priority)
            .ThenBy(o => o.ExpectedCompletionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOverdueOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.AssignedToUser)
            .Where(o => o.ExpectedCompletionDate < now &&
                       o.CurrentStatus != OrderStatus.Completed &&
                       o.CurrentStatus != OrderStatus.Closed &&
                       o.CurrentStatus != OrderStatus.Cancelled)
            .OrderBy(o => o.ExpectedCompletionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetSLABreachedOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.AssignedToUser)
            .Where(o => o.IsSLABreached)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MaintenanceOrder?> GetOrderWithDetailsAsync(
        long orderId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Hotel)
            .Include(o => o.RequestingDepartment)
            .Include(o => o.AssignedDepartment)
            .Include(o => o.Location)
            .Include(o => o.Item)
                .ThenInclude(i => i.Category)
            .Include(o => o.AssignedToUser)
            .Include(o => o.CreatedByUser)
            .Include(o => o.Vendor)
            .Include(o => o.StatusHistory)
                .ThenInclude(h => h.ChangedByUser)
            .Include(o => o.AssignmentHistory)
                .ThenInclude(h => h.AssignedByUser)
            .Include(o => o.Comments)
                .ThenInclude(c => c.User)
            .Include(o => o.Attachments)
                .ThenInclude(a => a.UploadedByUser)
            .Include(o => o.SparePartsUsed)
                .ThenInclude(s => s.SparePart)
            .Include(o => o.ChecklistItems)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }
}
