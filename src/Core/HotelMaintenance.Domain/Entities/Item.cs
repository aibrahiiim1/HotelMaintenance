using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents equipment or items in the hotel
/// </summary>
public class Item : AuditableEntity
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Classification
    public int CategoryId { get; set; }
    public int? ClassId { get; set; }
    public int? FamilyId { get; set; }
    
    // Location
    public int LocationId { get; set; }
    
    // Technical Details
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? AssetTag { get; set; }
    
    // Installation & Warranty
    public DateTime? InstallationDate { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public string? WarrantyProvider { get; set; }
    
    // Status
    public ItemStatus Status { get; set; } = ItemStatus.Operational;
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    
    // Specifications
    public string? Capacity { get; set; }
    public string? Power { get; set; }
    public string? Voltage { get; set; }
    public string? Specifications { get; set; }
    
    // Financial
    public decimal? PurchaseCost { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public int? SupplierVendorId { get; set; }
    public decimal? EstimatedLifeYears { get; set; }
    public decimal? CurrentValue { get; set; }
    
    // Documentation
    public string? ImageUrl { get; set; }
    public string? ManualUrl { get; set; }
    public string? QRCode { get; set; }
    public string? Barcode { get; set; }
    
    // Maintenance
    public bool RequiresPreventiveMaintenance { get; set; } = false;
    public int? MaintenanceIntervalDays { get; set; }
    
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsCritical { get; set; } = false;

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public ItemCategory Category { get; set; } = null!;
    public ItemClass? Class { get; set; }
    public ItemFamily? Family { get; set; }
    public Location Location { get; set; } = null!;
    public Vendor? SupplierVendor { get; set; }
    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
    public ICollection<PreventiveMaintenanceSchedule> PMSchedules { get; set; } = new List<PreventiveMaintenanceSchedule>();
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<ItemAttachment> Attachments { get; set; } = new List<ItemAttachment>();
}

/// <summary>
/// Attachments for items (photos, manuals, etc.)
/// </summary>
public class ItemAttachment : BaseEntity
{
    public int ItemId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public AttachmentType Type { get; set; }
    public string? Description { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }

    // Navigation Properties
    public Item Item { get; set; } = null!;
}
