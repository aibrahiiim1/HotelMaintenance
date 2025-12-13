namespace HotelMaintenance.Domain.Common;

/// <summary>
/// Base class for all entities with common properties
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}

/// <summary>
/// Base class for entities with audit trail
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public int? LastModifiedByUserId { get; set; }
}

/// <summary>
/// Base class for entities that can be soft-deleted
/// </summary>
public abstract class SoftDeletableEntity : AuditableEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
}
