using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.MaintenanceOrders
{
    public class MaintenanceOrderDetailDto
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

        // Collections - ADDED
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
        public List<OrderCommentDto> Comments { get; set; } = new();
        public List<OrderAttachmentDto> Attachments { get; set; } = new();

        // Counts
        public int AttachmentCount { get; set; }
        public int CommentCount { get; set; }
        public int SparePartsCount { get; set; }
    }
}
