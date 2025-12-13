using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a maintenance work order - the core entity of the system
/// </summary>
public class MaintenanceOrder : AuditableEntity
{
    public long Id { get; set; }
    
    /// <summary>
    /// Unique order number (e.g., MO-HTL001-2024-00001)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;
    
    // Hotel & Department Info
    public int HotelId { get; set; }
    public int RequestingDepartmentId { get; set; }
    public int? AssignedDepartmentId { get; set; }
    
    // Order Details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OrderPriority Priority { get; set; } = OrderPriority.Medium;
    public OrderType Type { get; set; } = OrderType.Corrective;
    
    // Location & Item
    public int LocationId { get; set; }
    public int? ItemId { get; set; }
    
    // Assignment
    public int? AssignedToUserId { get; set; }
    public AssignmentStatus AssignmentStatus { get; set; } = AssignmentStatus.NotAssigned;
    public DateTime? AssignedAt { get; set; }
    public int? AssignedByUserId { get; set; }
    
    // Status Management
    public OrderStatus CurrentStatus { get; set; } = OrderStatus.Draft;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime ExpectedCompletionDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    
    // External Work
    public bool IsExternalWork { get; set; } = false;
    public int? VendorId { get; set; }
    public string? ExternalWorkOrderNumber { get; set; }
    public DateTime? ExternalWorkStartDate { get; set; }
    public DateTime? ExternalWorkCompletionDate { get; set; }
    
    // Financial
    public decimal EstimatedCost { get; set; } = 0;
    public decimal ActualCost { get; set; } = 0;
    public decimal LaborCost { get; set; } = 0;
    public decimal MaterialCost { get; set; } = 0;
    public decimal ExternalCost { get; set; } = 0;
    public string? CostCenter { get; set; }
    public string? GLAccount { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    
    // SLA & Performance
    public DateTime? SLADeadline { get; set; }
    public bool IsSLABreached { get; set; } = false;
    public int? ResponseTimeMinutes { get; set; }
    public int? ResolutionTimeMinutes { get; set; }
    
    // Completion Details
    public string? ResolutionNotes { get; set; }
    public int? CompletedByUserId { get; set; }
    public bool RequiresFollowUp { get; set; } = false;
    public DateTime? FollowUpDate { get; set; }
    public long? FollowUpOrderId { get; set; }
    
    // Requester Feedback
    public bool IsApprovedByRequester { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedByUserId { get; set; }
    public int? Rating { get; set; }
    public string? RequesterFeedback { get; set; }
    
    // Cancellation
    public bool IsCancelled { get; set; } = false;
    public DateTime? CancelledAt { get; set; }
    public int? CancelledByUserId { get; set; }
    public string? CancellationReason { get; set; }
    
    // Rejection
    public bool IsRejected { get; set; } = false;
    public DateTime? RejectedAt { get; set; }
    public int? RejectedByUserId { get; set; }
    public string? RejectionReason { get; set; }
    
    // Additional Info
    public bool IsUrgent { get; set; } = false;
    public bool IsSafetyRelated { get; set; } = false;
    public bool IsGuestFacing { get; set; } = false;
    public string? GuestName { get; set; }
    public string? GuestRoomNumber { get; set; }
    
    /// <summary>
    /// Internal notes visible only to engineering
    /// </summary>
    public string? InternalNotes { get; set; }
    
    /// <summary>
    /// Tags for filtering/grouping (comma-separated)
    /// </summary>
    public string? Tags { get; set; }

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public Department RequestingDepartment { get; set; } = null!;
    public Department? AssignedDepartment { get; set; }
    public Location Location { get; set; } = null!;
    public Item? Item { get; set; }
    public User? AssignedToUser { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public User? AssignedByUser { get; set; }
    public User? CompletedByUser { get; set; }
    public User? ApprovedByUser { get; set; }
    public User? CancelledByUser { get; set; }
    public Vendor? Vendor { get; set; }
    public MaintenanceOrder? FollowUpOrder { get; set; }
    
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<OrderAssignmentHistory> AssignmentHistory { get; set; } = new List<OrderAssignmentHistory>();
    public ICollection<OrderSparePartUsage> SparePartsUsed { get; set; } = new List<OrderSparePartUsage>();
    public ICollection<OrderAttachment> Attachments { get; set; } = new List<OrderAttachment>();
    public ICollection<OrderComment> Comments { get; set; } = new List<OrderComment>();
    public ICollection<OrderChecklistItem> ChecklistItems { get; set; } = new List<OrderChecklistItem>();
}
