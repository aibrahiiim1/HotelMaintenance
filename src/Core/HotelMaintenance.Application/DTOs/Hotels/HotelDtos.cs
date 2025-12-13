namespace HotelMaintenance.Application.DTOs.Hotels;

public class HotelDto
{
    public int Id { get; set; }
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
    public bool IsActive { get; set; }
    public int? TotalRooms { get; set; }
    public int? StarRating { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHotelDto
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
    public int? ParentHotelId { get; set; }
    public int? TotalRooms { get; set; }
    public int? StarRating { get; set; }
}

public class UpdateHotelDto
{
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
    public bool IsActive { get; set; }
    public int? TotalRooms { get; set; }
    public int? StarRating { get; set; }
}
