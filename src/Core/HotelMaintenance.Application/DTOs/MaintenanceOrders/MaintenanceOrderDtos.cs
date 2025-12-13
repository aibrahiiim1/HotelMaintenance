using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Application.DTOs.MaintenanceOrders;

public class MaintenanceOrderDto
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    
    // Hotel & Department
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int RequestingDepartmentId { get; set; }
    public string RequestingDepartmentName { get; set; } = string.Empty;
    public int? AssignedDepartmentId { get; set; }
    public string? AssignedDepartmentName { get; set; }
    
    // Order Details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OrderPriority Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    
    // Location & Item
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int? ItemId { get; set; }
    public string? ItemName { get; set; }
    
    // Assignment
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public AssignmentStatus AssignmentStatus { get; set; }
    public string AssignmentStatusName { get; set; } = string.Empty;
    public DateTime? AssignedAt { get; set; }
    
    // Status
    public OrderStatus CurrentStatus { get; set; }
    public string CurrentStatusName { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime ExpectedCompletionDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    
    // External Work
    public bool IsExternalWork { get; set; }
    public int? VendorId { get; set; }
    public string? VendorName { get; set; }
    
    // Financial
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    
    // SLA
    public DateTime? SLADeadline { get; set; }
    public bool IsSLABreached { get; set; }
    public int? ResponseTimeMinutes { get; set; }
    public int? ResolutionTimeMinutes { get; set; }
    
    // Completion
    public string? ResolutionNotes { get; set; }
    public bool RequiresFollowUp { get; set; }
    public DateTime? FollowUpDate { get; set; }
    
    // Feedback
    public bool IsApprovedByRequester { get; set; }
    public int? Rating { get; set; }
    public string? RequesterFeedback { get; set; }
    
    // Flags
    public bool IsCancelled { get; set; }
    public bool IsRejected { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsSafetyRelated { get; set; }
    public bool IsGuestFacing { get; set; }
    
    // Audit
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    
    // Counts
    public int AttachmentCount { get; set; }
    public int CommentCount { get; set; }
    public int SparePartsCount { get; set; }
}

public class CreateMaintenanceOrderDto
{
    public int HotelId { get; set; }
    public int RequestingDepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OrderPriority Priority { get; set; } = OrderPriority.Medium;
    public OrderType Type { get; set; } = OrderType.Corrective;
    public int LocationId { get; set; }
    public int? ItemId { get; set; }
    public DateTime ExpectedCompletionDate { get; set; }
    public bool IsUrgent { get; set; } = false;
    public bool IsSafetyRelated { get; set; } = false;
    public bool IsGuestFacing { get; set; } = false;
    public string? GuestName { get; set; }
    public string? GuestRoomNumber { get; set; }
    public string? Tags { get; set; }
}

public class UpdateMaintenanceOrderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OrderPriority Priority { get; set; }
    public int LocationId { get; set; }
    public int? ItemId { get; set; }
    public DateTime ExpectedCompletionDate { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsSafetyRelated { get; set; }
}

public class AssignOrderDto
{
    public int AssignedDepartmentId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? Reason { get; set; }
}

public class UpdateOrderStatusDto
{
    public OrderStatus NewStatus { get; set; }
    public string? Notes { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class CompleteOrderDto
{
    public string ResolutionNotes { get; set; } = string.Empty;
    public bool RequiresFollowUp { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public List<SparePartUsageDto> SparePartsUsed { get; set; } = new();
    public decimal LaborCost { get; set; }
    public decimal MaterialCost { get; set; }
}

public class SparePartUsageDto
{
    public int SparePartId { get; set; }
    public int QuantityUsed { get; set; }
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }
}

public class VerifyOrderDto
{
    public int Rating { get; set; }
    public string? Feedback { get; set; }
    public bool IsApproved { get; set; }
}

public class CancelOrderDto
{
    public string CancellationReason { get; set; } = string.Empty;
}

public class MaintenanceOrderFilterDto
{
    public int? HotelId { get; set; }
    public int? DepartmentId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? LocationId { get; set; }
    public int? ItemId { get; set; }
    public OrderStatus? Status { get; set; }
    public OrderPriority? Priority { get; set; }
    public OrderType? Type { get; set; }
    public AssignmentStatus? AssignmentStatus { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsSLABreached { get; set; }
    public bool? IsUrgent { get; set; }
    public bool? IsSafetyRelated { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

public class OrderStatusHistoryDto
{
    public long Id { get; set; }
    public OrderStatus FromStatus { get; set; }
    public string FromStatusName { get; set; } = string.Empty;
    public OrderStatus ToStatus { get; set; }
    public string ToStatusName { get; set; } = string.Empty;
    public string ChangedByUserName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
}

public class OrderAssignmentHistoryDto
{
    public long Id { get; set; }
    public string? FromDepartmentName { get; set; }
    public string? ToDepartmentName { get; set; }
    public string? FromUserName { get; set; }
    public string? ToUserName { get; set; }
    public string AssignedByUserName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string? Reason { get; set; }
}

public class OrderCommentDto
{
    public long Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsEdited { get; set; }
}

public class CreateOrderCommentDto
{
    public long MaintenanceOrderId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
}

public class OrderAttachmentDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public AttachmentType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UploadedByUserName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
