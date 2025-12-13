using HotelMaintenance.Domain.Common;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a hotel in the hotel group
/// </summary>
public class Hotel : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// For hotel chains - parent hotel group
    /// </summary>
    public int? ParentHotelId { get; set; }
    
    /// <summary>
    /// Number of rooms in the hotel
    /// </summary>
    public int? TotalRooms { get; set; }
    
    /// <summary>
    /// Hotel star rating
    /// </summary>
    public int? StarRating { get; set; }
    
    /// <summary>
    /// Logo image URL
    /// </summary>
    public string? LogoUrl { get; set; }

    // Navigation Properties
    public Hotel? ParentHotel { get; set; }
    public ICollection<Hotel> ChildHotels { get; set; } = new List<Hotel>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
