using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a physical location within a hotel
/// </summary>
public class Location : AuditableEntity
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LocationType Type { get; set; }
    
    /// <summary>
    /// Parent location for hierarchical structure (Building > Floor > Room)
    /// </summary>
    public int? ParentLocationId { get; set; }
    
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Zone { get; set; }
    public string? RoomNumber { get; set; }
    
    /// <summary>
    /// GPS coordinates if available
    /// </summary>
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// Square footage/area
    /// </summary>
    public decimal? Area { get; set; }
    public string? AreaUnit { get; set; }
    
    /// <summary>
    /// Access instructions or notes
    /// </summary>
    public string? AccessInstructions { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public Location? ParentLocation { get; set; }
    public ICollection<Location> SubLocations { get; set; } = new List<Location>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
}
