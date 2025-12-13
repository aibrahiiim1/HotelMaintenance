using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents external vendors/contractors
/// </summary>
public class Vendor : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public VendorType Type { get; set; }
    public string? Description { get; set; }
    
    // Contact Information
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Website { get; set; }
    
    // Address
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    
    // Business Details
    public string? TaxId { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? InsurancePolicy { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    
    // Performance
    public bool IsPreferred { get; set; } = false;
    public decimal? ServiceRating { get; set; }
    public int TotalOrdersCompleted { get; set; } = 0;
    public decimal? AverageResponseTime { get; set; }
    public decimal? OnTimeCompletionRate { get; set; }
    
    // Payment Terms
    public string? PaymentTerms { get; set; }
    public int? PaymentDueDays { get; set; }
    
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation Properties
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<Item> SuppliedItems { get; set; } = new List<Item>();
    public ICollection<SparePart> SuppliedParts { get; set; } = new List<SparePart>();
    public ICollection<VendorContract> Contracts { get; set; } = new List<VendorContract>();
}

/// <summary>
/// Vendor service contracts
/// </summary>
public class VendorContract : BaseEntity
{
    public int VendorId { get; set; }
    public int HotelId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal ContractValue { get; set; }
    public string? PaymentSchedule { get; set; }
    public string? ServiceScope { get; set; }
    public string? SLATerms { get; set; }
    public bool IsActive { get; set; } = true;
    public string? DocumentUrl { get; set; }

    // Navigation Properties
    public Vendor Vendor { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
}
