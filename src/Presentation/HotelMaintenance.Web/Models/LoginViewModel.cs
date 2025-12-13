using System.ComponentModel.DataAnnotations;

namespace HotelMaintenance.Web.Models;

/// <summary>
/// Login view model
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Dashboard statistics view model
/// </summary>
public class DashboardViewModel
{
    public int TotalOrders { get; set; }
    public int OpenOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int OverdueOrders { get; set; }
    public int SLABreachedOrders { get; set; }
    public decimal AverageCost { get; set; }
    public decimal TotalCost { get; set; }

    public OrdersByPriorityViewModel OrdersByPriority { get; set; } = new();
    public List<OrderStatusCountViewModel> OrdersByStatus { get; set; } = new();
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    public List<OrderTrendViewModel> OrderTrends { get; set; } = new();
}

public class OrdersByPriorityViewModel
{
    public int Critical { get; set; }
    public int High { get; set; }
    public int Medium { get; set; }
    public int Low { get; set; }
}

public class OrderStatusCountViewModel
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentOrderViewModel
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string? AssignedToUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderTrendViewModel
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public int Completed { get; set; }
}

/// <summary>
/// Maintenance order list view model
/// </summary>
public class MaintenanceOrderListViewModel
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string? LocationName { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public bool IsSLABreached { get; set; }
}

/// <summary>
/// Maintenance order detail view model
/// </summary>
public class MaintenanceOrderDetailViewModel
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string AssignmentStatus { get; set; } = string.Empty;

    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int RequestingDepartmentId { get; set; }
    public string RequestingDepartmentName { get; set; } = string.Empty;
    public int? AssignedDepartmentId { get; set; }
    public string? AssignedDepartmentName { get; set; }
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
    public int? ItemId { get; set; }
    public string? ItemName { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;

    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSLABreached { get; set; }

    public List<OrderStatusHistoryViewModel> StatusHistory { get; set; } = new();
    public List<OrderCommentViewModel> Comments { get; set; } = new();
    public List<OrderAttachmentViewModel> Attachments { get; set; } = new();
}

public class OrderStatusHistoryViewModel
{
    public int Id { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ChangedByUserName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class OrderCommentViewModel
{
    public int Id { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}

public class OrderAttachmentViewModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedByUserName { get; set; } = string.Empty;
}

/// <summary>
/// Create/Edit order view model
/// </summary>
public class CreateOrderViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int Priority { get; set; }

    [Required]
    public int Type { get; set; }

    [Required]
    public int HotelId { get; set; }

    [Required]
    public int RequestingDepartmentId { get; set; }

    public int? LocationId { get; set; }
    public int? ItemId { get; set; }

    public DateTime? ExpectedCompletionDate { get; set; }

    public decimal? EstimatedCost { get; set; }

    [StringLength(200)]
    public string? GuestName { get; set; }

    [StringLength(20)]
    public string? GuestRoomNumber { get; set; }

    [StringLength(2000)]
    public string? InternalNotes { get; set; }
}

/// <summary>
/// User information from JWT
/// </summary>
public class UserInfoViewModel
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
}