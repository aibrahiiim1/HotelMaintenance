using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Tracks all status changes for a maintenance order
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
    public string? SystemNotes { get; set; }

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}

/// <summary>
/// Tracks all assignment changes for a maintenance order
/// </summary>
public class OrderAssignmentHistory : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public int? FromDepartmentId { get; set; }
    public int? ToDepartmentId { get; set; }
    public int? FromUserId { get; set; }
    public int? ToUserId { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedByUserId { get; set; }
    public string? Reason { get; set; }

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public Department? FromDepartment { get; set; }
    public Department? ToDepartment { get; set; }
    public User? FromUser { get; set; }
    public User? ToUser { get; set; }
    public User AssignedByUser { get; set; } = null!;
}

/// <summary>
/// Comments and notes on maintenance orders
/// </summary>
public class OrderComment : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public int UserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    
    /// <summary>
    /// Internal comments are visible only to engineering department
    /// </summary>
    public bool IsInternal { get; set; } = false;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsEdited { get; set; } = false;

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public User User { get; set; } = null!;
}

/// <summary>
/// File attachments for maintenance orders (photos, documents, etc.)
/// </summary>
public class OrderAttachment : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public AttachmentType Type { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? Description { get; set; }
    
    /// <summary>
    /// Is this a before or after photo?
    /// </summary>
    public string? PhotoStage { get; set; }

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public User UploadedByUser { get; set; } = null!;
}

/// <summary>
/// Checklist items for maintenance orders
/// </summary>
public class OrderChecklistItem : BaseEntity
{
    public long Id { get; set; }
    public long MaintenanceOrderId { get; set; }
    public int Sequence { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public CheckItemType Type { get; set; } = CheckItemType.Checkbox;
    public bool IsRequired { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public string? CompletionValue { get; set; }
    public int? CompletedByUserId { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation Properties
    public MaintenanceOrder Order { get; set; } = null!;
    public User? CompletedByUser { get; set; }
}
