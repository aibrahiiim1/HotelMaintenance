using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents spare parts inventory
/// </summary>
public class SparePart : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Classification
    public int? ItemId { get; set; }
    public int? CategoryId { get; set; }
    public string? PartNumber { get; set; }
    public string? Manufacturer { get; set; }
    
    // Storage
    public int HotelId { get; set; }
    public int StorageDepartmentId { get; set; }
    public string? StorageLocation { get; set; }
    public string? BinLocation { get; set; }
    
    // Inventory
    public int QuantityOnHand { get; set; }
    public int MinimumQuantity { get; set; }
    public int ReorderQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    
    // Financial
    public decimal UnitCost { get; set; }
    public decimal? LastPurchasePrice { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public int? PreferredVendorId { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsCritical { get; set; } = false;
    
    /// <summary>
    /// Image URL for the spare part
    /// </summary>
    public string? ImageUrl { get; set; }
    
    public string? Notes { get; set; }

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemCategory? Category { get; set; }
    public Department StorageDepartment { get; set; } = null!;
    public Vendor? PreferredVendor { get; set; }
    public ICollection<OrderSparePartUsage> OrderUsages { get; set; } = new List<OrderSparePartUsage>();
    public ICollection<SparePartTransaction> Transactions { get; set; } = new List<SparePartTransaction>();
}

/// <summary>
/// Tracks spare part usage in maintenance orders
/// </summary>
public class OrderSparePartUsage : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public int SparePartId { get; set; }
    public int QuantityUsed { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public int UsedByUserId { get; set; }
    public DateTime UsedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public SparePart SparePart { get; set; } = null!;
    public User UsedByUser { get; set; } = null!;
}

/// <summary>
/// Tracks all spare part inventory transactions
/// </summary>
public class SparePartTransaction : BaseEntity
{
    public long Id { get; set; }
    public int SparePartId { get; set; }
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    
    /// <summary>
    /// Reference to related entity (Order ID, PO ID, etc.)
    /// </summary>
    public long? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceNumber { get; set; }
    
    public int TransactionByUserId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Notes { get; set; }
    
    /// <summary>
    /// For transfers between locations
    /// </summary>
    public int? FromDepartmentId { get; set; }
    public int? ToDepartmentId { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }

    // Navigation Properties
    public SparePart SparePart { get; set; } = null!;
    public User TransactionByUser { get; set; } = null!;
    public Department? FromDepartment { get; set; }
    public Department? ToDepartment { get; set; }
}
