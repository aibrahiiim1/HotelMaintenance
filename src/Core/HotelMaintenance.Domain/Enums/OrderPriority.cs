namespace HotelMaintenance.Domain.Enums;

/// <summary>
/// Priority levels for maintenance orders
/// </summary>
public enum OrderPriority
{
    /// <summary>
    /// Critical - Immediate attention required (Safety/Revenue impact)
    /// SLA: Response within 15 minutes, Resolution within 2 hours
    /// </summary>
    Critical = 1,

    /// <summary>
    /// High - Same day resolution required
    /// SLA: Response within 1 hour, Resolution within 8 hours
    /// </summary>
    High = 2,

    /// <summary>
    /// Medium - Resolution within 1-3 days
    /// SLA: Response within 4 hours, Resolution within 24 hours
    /// </summary>
    Medium = 3,

    /// <summary>
    /// Low - Resolution within 4-7 days
    /// SLA: Response within 24 hours, Resolution within 7 days
    /// </summary>
    Low = 4
}
