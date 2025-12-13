using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Application.DTOs.Departments;

public class DepartmentDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DepartmentType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public bool CanCreateOrders { get; set; }
    public bool CanReceiveOrders { get; set; }
    public bool IsMaintenanceProvider { get; set; }
    public int? ManagerUserId { get; set; }
    public string? ManagerName { get; set; }
    public int? DefaultResponseTimeMinutes { get; set; }
    public string? CostCenter { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDepartmentDto
{
    public int HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DepartmentType Type { get; set; }
    public bool CanCreateOrders { get; set; } = true;
    public bool CanReceiveOrders { get; set; } = false;
    public bool IsMaintenanceProvider { get; set; } = false;
    public int? ManagerUserId { get; set; }
    public int? DefaultResponseTimeMinutes { get; set; }
    public string? CostCenter { get; set; }
}

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DepartmentType Type { get; set; }
    public bool CanCreateOrders { get; set; }
    public bool CanReceiveOrders { get; set; }
    public bool IsMaintenanceProvider { get; set; }
    public int? ManagerUserId { get; set; }
    public int? DefaultResponseTimeMinutes { get; set; }
    public string? CostCenter { get; set; }
    public bool IsActive { get; set; }
}
