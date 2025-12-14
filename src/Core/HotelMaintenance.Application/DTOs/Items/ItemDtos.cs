using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Application.DTOs.Locations;

public class LocationDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LocationType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int? ParentLocationId { get; set; }
    public string? ParentLocationName { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Zone { get; set; }
    public string? RoomNumber { get; set; }
    public bool IsActive { get; set; }
}

public class CreateLocationDto
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LocationType Type { get; set; }
    public int? ParentLocationId { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Zone { get; set; }
    public string? RoomNumber { get; set; }
}
public class UpdateLocationDto
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LocationType Type { get; set; }
    public int? ParentLocationId { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Zone { get; set; }
    public string? RoomNumber { get; set; }
}


public class ItemDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int? FamilyId { get; set; }
    public string? FamilyName { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public ItemStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsCritical { get; set; }
    public bool IsActive { get; set; }
}

public class CreateItemDto
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int? ClassId { get; set; }
    public int? FamilyId { get; set; }
    public int LocationId { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? AssetTag { get; set; }
    public DateTime? InstallationDate { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public bool RequiresPreventiveMaintenance { get; set; }
    public int? MaintenanceIntervalDays { get; set; }
    public bool IsCritical { get; set; }
}
public class UpdateItemDto
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int? ClassId { get; set; }
    public int? FamilyId { get; set; }
    public int LocationId { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? AssetTag { get; set; }
    public DateTime? InstallationDate { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public bool RequiresPreventiveMaintenance { get; set; }
    public int? MaintenanceIntervalDays { get; set; }
    public bool IsCritical { get; set; }
    public ItemStatus Status { get; set; }
}

public class ItemCategoryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
}

public class ItemClassDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ItemFamilyDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
