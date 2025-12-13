using HotelMaintenance.Domain.Common;

namespace HotelMaintenance.Domain.Entities;

/// <summary>
/// Represents the top-level category of equipment
/// Example: HVAC, Electrical, Plumbing, Kitchen Equipment
/// </summary>
public class ItemCategory : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<ItemClass> Classes { get; set; } = new List<ItemClass>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
}

/// <summary>
/// Represents a sub-category within a category
/// Example: Air Conditioner, Water Heater, Refrigerator
/// </summary>
public class ItemClass : BaseEntity
{
    public int CategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ItemCategory Category { get; set; } = null!;
    public ICollection<ItemFamily> Families { get; set; } = new List<ItemFamily>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
}

/// <summary>
/// Represents the specific type within a class
/// Example: Split AC, Central AC, Window AC
/// </summary>
public class ItemFamily : BaseEntity
{
    public int ClassId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ItemClass Class { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
