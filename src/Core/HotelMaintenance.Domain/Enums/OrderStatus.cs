namespace HotelMaintenance.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a maintenance order
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is being created but not yet submitted
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Order submitted and awaiting assignment to a technician
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Order has been assigned to a technician
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Work has started on the order
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Work is temporarily paused
    /// </summary>
    OnHold = 4,

    /// <summary>
    /// Waiting for spare parts to arrive
    /// </summary>
    AwaitingParts = 5,

    /// <summary>
    /// Work has been sent to an external vendor/contractor
    /// </summary>
    ExternalWork = 6,

    /// <summary>
    /// Work is scheduled for a future date
    /// </summary>
    Scheduled = 7,

    /// <summary>
    /// Work has been completed by engineering
    /// </summary>
    Completed = 8,

    /// <summary>
    /// Work has been verified/approved by the requester
    /// </summary>
    Verified = 9,

    /// <summary>
    /// Order is closed and locked (final state)
    /// </summary>
    Closed = 10,

    /// <summary>
    /// Order was cancelled before completion
    /// </summary>
    Cancelled = 11,

    /// <summary>
    /// Order was rejected or unable to be completed
    /// </summary>
    Rejected = 12,

    /// <summary>
    /// Order was reopened after being completed
    /// </summary>
    Reopened = 13
}
