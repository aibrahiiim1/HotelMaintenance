using HotelMaintenance.Domain.Common;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents a role in the system
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Junction table for User-Role many-to-many relationship
/// </summary>
public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public int? HotelId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedByUserId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Hotel? Hotel { get; set; }
    public Department? Department { get; set; }
}

/// <summary>
/// Represents a permission in the system
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Junction table for Role-Permission many-to-many relationship
/// </summary>
public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
