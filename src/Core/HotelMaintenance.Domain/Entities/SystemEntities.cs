using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// SLA (Service Level Agreement) configuration for maintenance orders
/// </summary>
public class SLAConfiguration : BaseEntity
{
    public int HotelId { get; set; }
    public OrderPriority Priority { get; set; }
    
    /// <summary>
    /// Time to assign order in minutes
    /// </summary>
    public int ResponseTimeMinutes { get; set; }
    
    /// <summary>
    /// Time to complete order in minutes
    /// </summary>
    public int ResolutionTimeMinutes { get; set; }
    
    /// <summary>
    /// Send escalation if SLA is about to breach
    /// </summary>
    public bool EnableEscalation { get; set; } = true;
    
    /// <summary>
    /// Minutes before SLA breach to send warning
    /// </summary>
    public int EscalationThresholdMinutes { get; set; } = 30;
    
    /// <summary>
    /// User IDs to notify on escalation (comma-separated)
    /// </summary>
    public string? EscalationUserIds { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
}

/// <summary>
/// Notification templates
/// </summary>
public class NotificationTemplate : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum NotificationType
{
    Email = 1,
    SMS = 2,
    Push = 3,
    InApp = 4
}

/// <summary>
/// Notification log
/// </summary>
public class NotificationLog : BaseEntity
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public long? MaintenanceOrderId { get; set; }
    public NotificationType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool WasSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RecipientAddress { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public MaintenanceOrder? MaintenanceOrder { get; set; }
}

/// <summary>
/// System settings and configuration
/// </summary>
public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public string? Category { get; set; }
    public bool IsEncrypted { get; set; } = false;
}

/// <summary>
/// Audit log for important system actions
/// </summary>
public class AuditLog : BaseEntity
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Navigation Properties
    public User? User { get; set; }
}
