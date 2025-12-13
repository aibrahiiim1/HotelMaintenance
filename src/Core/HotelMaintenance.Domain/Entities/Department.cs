using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a department within a hotel
/// </summary>
public class Department : AuditableEntity
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DepartmentType Type { get; set; }
    
    /// <summary>
    /// Can this department create maintenance orders?
    /// </summary>
    public bool CanCreateOrders { get; set; } = true;
    
    /// <summary>
    /// Can this department receive and work on maintenance orders?
    /// </summary>
    public bool CanReceiveOrders { get; set; } = false;
    
    /// <summary>
    /// Is this the maintenance/engineering department?
    /// </summary>
    public bool IsMaintenanceProvider { get; set; } = false;
    
    /// <summary>
    /// Department manager
    /// </summary>
    public int? ManagerUserId { get; set; }
    
    /// <summary>
    /// Default response time for orders in minutes
    /// </summary>
    public int? DefaultResponseTimeMinutes { get; set; }
    
    /// <summary>
    /// Cost center code for financial tracking
    /// </summary>
    public string? CostCenter { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public User? Manager { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<MaintenanceOrder> RequestedOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<MaintenanceOrder> AssignedOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
}
