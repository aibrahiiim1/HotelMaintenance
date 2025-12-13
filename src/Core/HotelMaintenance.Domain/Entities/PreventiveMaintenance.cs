using HotelMaintenance.Domain.Common;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Preventive maintenance schedules for equipment
/// </summary>
public class PreventiveMaintenanceSchedule : AuditableEntity
{
    public int ItemId { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProcedureNotes { get; set; }
    
    // Frequency
    public PMFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; }
    
    // Dates
    public DateTime LastPerformedDate { get; set; }
    public DateTime NextDueDate { get; set; }
    
    // Work Details
    public int EstimatedDurationMinutes { get; set; }
    public int AssignedDepartmentId { get; set; }
    public int? DefaultAssignedUserId { get; set; }
    
    // Automation
    public bool AutoGenerateOrders { get; set; } = true;
    public int DaysBeforeDueToGenerate { get; set; } = 7;
    
    // Checklist
    public int? ChecklistTemplateId { get; set; }
    
    public OrderPriority DefaultPriority { get; set; } = OrderPriority.Low;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Item Item { get; set; } = null!;
    public Department AssignedDepartment { get; set; } = null!;
    public User? DefaultAssignedUser { get; set; }
    public ChecklistTemplate? ChecklistTemplate { get; set; }
    public ICollection<PMScheduleHistory> ScheduleHistory { get; set; } = new List<PMScheduleHistory>();
}

/// <summary>
/// History of preventive maintenance executions
/// </summary>
public class PMScheduleHistory : BaseEntity
{
    public long Id { get; set; }
    public int PMScheduleId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public long? MaintenanceOrderId { get; set; }
    public bool WasCompleted { get; set; } = false;
    public string? Notes { get; set; }

    // Navigation Properties
    public PreventiveMaintenanceSchedule PMSchedule { get; set; } = null!;
    public MaintenanceOrder? MaintenanceOrder { get; set; }
}

/// <summary>
/// Checklist templates for maintenance procedures
/// </summary>
public class ChecklistTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChecklistType Type { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ItemCategory? Category { get; set; }
    public ICollection<ChecklistTemplateItem> Items { get; set; } = new List<ChecklistTemplateItem>();
}

/// <summary>
/// Individual items within a checklist template
/// </summary>
public class ChecklistTemplateItem : BaseEntity
{
    public int TemplateId { get; set; }
    public int Sequence { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public CheckItemType Type { get; set; } = CheckItemType.Checkbox;
    public bool IsRequired { get; set; } = false;
    public string? ExpectedValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? HelpText { get; set; }

    // Navigation Properties
    public ChecklistTemplate Template { get; set; } = null!;
}
