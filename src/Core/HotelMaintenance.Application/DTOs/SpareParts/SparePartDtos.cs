namespace HotelMaintenance.Application.DTOs.SpareParts;

public class SparePartDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? PartNumber { get; set; }
    public string? Manufacturer { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int StorageDepartmentId { get; set; }
    public string StorageDepartmentName { get; set; } = string.Empty;
    public string? StorageLocation { get; set; }
    public int QuantityOnHand { get; set; }
    public int MinimumQuantity { get; set; }
    public int ReorderQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; }
    public bool IsCritical { get; set; }
    public bool IsLowStock => QuantityOnHand <= MinimumQuantity;
}

public class CreateSparePartDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ItemId { get; set; }
    public string? PartNumber { get; set; }
    public string? Manufacturer { get; set; }
    public int HotelId { get; set; }
    public int StorageDepartmentId { get; set; }
    public string? StorageLocation { get; set; }
    public int QuantityOnHand { get; set; }
    public int MinimumQuantity { get; set; }
    public int ReorderQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    public decimal UnitCost { get; set; }
    public bool IsCritical { get; set; }
}

public class UpdateSparePartDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PartNumber { get; set; }
    public string? StorageLocation { get; set; }
    public int MinimumQuantity { get; set; }
    public int ReorderQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; }
}

public class AdjustSparePartQuantityDto
{
    public int SparePartId { get; set; }
    public int QuantityChange { get; set; }
    public string? Reason { get; set; }
}
