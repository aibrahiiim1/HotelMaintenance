using HotelMaintenance.Domain.Common;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : AuditableEntity
{
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    
    /// <summary>
    /// Primary hotel assignment
    /// </summary>
    public int HotelId { get; set; }
    
    /// <summary>
    /// Primary department assignment
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// Job title
    /// </summary>
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Profile photo URL
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Is the user currently available for work assignment?
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// Is the user active in the system?
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Preferred language code (en, ar, fr, etc.)
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";
    
    /// <summary>
    /// User's time zone
    /// </summary>
    public string? TimeZone { get; set; }
    
    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Receive email notifications
    /// </summary>
    public bool EmailNotifications { get; set; } = true;
    
    /// <summary>
    /// Receive SMS notifications
    /// </summary>
    public bool SmsNotifications { get; set; } = false;
    
    /// <summary>
    /// Receive push notifications
    /// </summary>
    public bool PushNotifications { get; set; } = true;

    // Computed Property
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public Hotel Hotel { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<MaintenanceOrder> CreatedOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<MaintenanceOrder> AssignedOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<Department> ManagedDepartments { get; set; } = new List<Department>();
}
