namespace HotelMaintenance.Domain.Enums;

/// <summary>
/// Type of maintenance order
/// </summary>
public enum OrderType
{
    /// <summary>
    /// Corrective maintenance - fixing broken equipment
    /// </summary>
    Corrective = 1,

    /// <summary>
    /// Preventive maintenance - scheduled routine maintenance
    /// </summary>
    Preventive = 2,

    /// <summary>
    /// Predictive maintenance - based on condition monitoring
    /// </summary>
    Predictive = 3,

    /// <summary>
    /// Emergency maintenance - urgent breakdown
    /// </summary>
    Emergency = 4,

    /// <summary>
    /// Inspection - routine inspection
    /// </summary>
    Inspection = 5,

    /// <summary>
    /// Upgrade/Modification
    /// </summary>
    Upgrade = 6
}

/// <summary>
/// Assignment status of an order
/// </summary>
public enum AssignmentStatus
{
    /// <summary>
    /// Order not yet assigned to any technician
    /// </summary>
    NotAssigned = 0,

    /// <summary>
    /// Order assigned to a technician
    /// </summary>
    Assigned = 1,

    /// <summary>
    /// Order reassigned to different technician
    /// </summary>
    Reassigned = 2,

    /// <summary>
    /// Assignment is pending approval
    /// </summary>
    PendingApproval = 3
}

/// <summary>
/// Department types in a hotel
/// </summary>
public enum DepartmentType
{
    Engineering = 1,
    Housekeeping = 2,
    FoodAndBeverage = 3,
    FrontOffice = 4,
    Security = 5,
    Sales = 6,
    HumanResources = 7,
    Finance = 8,
    Spa = 9,
    Recreation = 10,
    Kitchen = 11,
    Laundry = 12,
    Purchasing = 13,
    IT = 14,
    Management = 15
}

/// <summary>
/// Location types in a hotel
/// </summary>
public enum LocationType
{
    GuestRoom = 1,
    PublicArea = 2,
    BackOfHouse = 3,
    Mechanical = 4,
    Restaurant = 5,
    Bar = 6,
    Lobby = 7,
    Conference = 8,
    Pool = 9,
    Gym = 10,
    Spa = 11,
    Kitchen = 12,
    Office = 13,
    Storage = 14,
    Laundry = 15,
    Parking = 16,
    Exterior = 17,
    Roof = 18,
    Basement = 19
}

/// <summary>
/// Status of equipment/items
/// </summary>
public enum ItemStatus
{
    /// <summary>
    /// Item is operational and in use
    /// </summary>
    Operational = 1,

    /// <summary>
    /// Item is under maintenance
    /// </summary>
    UnderMaintenance = 2,

    /// <summary>
    /// Item is broken and not operational
    /// </summary>
    Broken = 3,

    /// <summary>
    /// Item is retired/decommissioned
    /// </summary>
    Retired = 4,

    /// <summary>
    /// Item is in storage
    /// </summary>
    InStorage = 5,

    /// <summary>
    /// Item is on order/not yet received
    /// </summary>
    OnOrder = 6
}

/// <summary>
/// Types of vendors
/// </summary>
public enum VendorType
{
    HVAC = 1,
    Electrical = 2,
    Plumbing = 3,
    Carpentry = 4,
    Painting = 5,
    Landscaping = 6,
    Cleaning = 7,
    Security = 8,
    IT = 9,
    FireSafety = 10,
    Elevator = 11,
    Pool = 12,
    Kitchen = 13,
    Laundry = 14,
    General = 15
}

/// <summary>
/// Preventive maintenance frequency
/// </summary>
public enum PMFrequency
{
    Daily = 1,
    Weekly = 2,
    Biweekly = 3,
    Monthly = 4,
    Quarterly = 5,
    SemiAnnually = 6,
    Annually = 7,
    Custom = 8
}

/// <summary>
/// Checklist types
/// </summary>
public enum ChecklistType
{
    Safety = 1,
    PreventiveMaintenance = 2,
    Inspection = 3,
    Commissioning = 4,
    Decommissioning = 5,
    Quality = 6
}

/// <summary>
/// Checklist item types
/// </summary>
public enum CheckItemType
{
    Checkbox = 1,
    Text = 2,
    Numeric = 3,
    Date = 4,
    Photo = 5,
    Signature = 6
}

/// <summary>
/// Attachment types
/// </summary>
public enum AttachmentType
{
    Photo = 1,
    Document = 2,
    Video = 3,
    Audio = 4,
    Other = 5
}

/// <summary>
/// Transaction types for spare parts
/// </summary>
public enum TransactionType
{
    Purchase = 1,
    Usage = 2,
    Adjustment = 3,
    Transfer = 4,
    Return = 5,
    Disposal = 6
}
