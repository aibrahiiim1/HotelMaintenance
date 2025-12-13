using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HotelMaintenance.API.Hubs;

/// <summary>
/// SignalR hub for real-time notifications
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("User {UserId} connected to NotificationHub. ConnectionId: {ConnectionId}", 
            userId, Context.ConnectionId);
        
        // Add to user-specific group
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("User {UserId} disconnected from NotificationHub. ConnectionId: {ConnectionId}", 
            userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a hotel group to receive hotel-specific notifications
    /// </summary>
    public async Task JoinHotelGroup(int hotelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"hotel_{hotelId}");
        _logger.LogInformation("Connection {ConnectionId} joined hotel group {HotelId}", 
            Context.ConnectionId, hotelId);
    }

    /// <summary>
    /// Leave a hotel group
    /// </summary>
    public async Task LeaveHotelGroup(int hotelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"hotel_{hotelId}");
        _logger.LogInformation("Connection {ConnectionId} left hotel group {HotelId}", 
            Context.ConnectionId, hotelId);
    }

    /// <summary>
    /// Join a department group
    /// </summary>
    public async Task JoinDepartmentGroup(int departmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"department_{departmentId}");
        _logger.LogInformation("Connection {ConnectionId} joined department group {DepartmentId}", 
            Context.ConnectionId, departmentId);
    }

    /// <summary>
    /// Leave a department group
    /// </summary>
    public async Task LeaveDepartmentGroup(int departmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"department_{departmentId}");
        _logger.LogInformation("Connection {ConnectionId} left department group {DepartmentId}", 
            Context.ConnectionId, departmentId);
    }
}

/// <summary>
/// Service for sending notifications through SignalR
/// </summary>
public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Send notification to a specific user
    /// </summary>
    public async Task SendToUserAsync(int userId, string type, object data)
    {
        try
        {
            await _hubContext.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new
                {
                    type,
                    data,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogInformation("Notification sent to user {UserId}. Type: {Type}", userId, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    /// <summary>
    /// Send notification to all users in a hotel
    /// </summary>
    public async Task SendToHotelAsync(int hotelId, string type, object data)
    {
        try
        {
            await _hubContext.Clients.Group($"hotel_{hotelId}")
                .SendAsync("ReceiveNotification", new
                {
                    type,
                    data,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogInformation("Notification sent to hotel {HotelId}. Type: {Type}", hotelId, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to hotel {HotelId}", hotelId);
        }
    }

    /// <summary>
    /// Send notification to all users in a department
    /// </summary>
    public async Task SendToDepartmentAsync(int departmentId, string type, object data)
    {
        try
        {
            await _hubContext.Clients.Group($"department_{departmentId}")
                .SendAsync("ReceiveNotification", new
                {
                    type,
                    data,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogInformation("Notification sent to department {DepartmentId}. Type: {Type}", 
                departmentId, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to department {DepartmentId}", departmentId);
        }
    }

    /// <summary>
    /// Send notification to all connected clients
    /// </summary>
    public async Task SendToAllAsync(string type, object data)
    {
        try
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", new
                {
                    type,
                    data,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogInformation("Notification sent to all clients. Type: {Type}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to all clients");
        }
    }

    /// <summary>
    /// Notify about new order
    /// </summary>
    public async Task NotifyNewOrderAsync(int hotelId, int departmentId, object orderData)
    {
        await SendToHotelAsync(hotelId, "NEW_ORDER", orderData);
        await SendToDepartmentAsync(departmentId, "NEW_ORDER", orderData);
    }

    /// <summary>
    /// Notify about order assignment
    /// </summary>
    public async Task NotifyOrderAssignedAsync(int userId, object orderData)
    {
        await SendToUserAsync(userId, "ORDER_ASSIGNED", orderData);
    }

    /// <summary>
    /// Notify about order status change
    /// </summary>
    public async Task NotifyOrderStatusChangedAsync(int hotelId, object orderData)
    {
        await SendToHotelAsync(hotelId, "ORDER_STATUS_CHANGED", orderData);
    }

    /// <summary>
    /// Notify about SLA breach
    /// </summary>
    public async Task NotifySLABreachAsync(int hotelId, int userId, object orderData)
    {
        await SendToHotelAsync(hotelId, "SLA_BREACH", orderData);
        await SendToUserAsync(userId, "SLA_BREACH", orderData);
    }

    /// <summary>
    /// Notify about low stock spare parts
    /// </summary>
    public async Task NotifyLowStockAsync(int hotelId, object sparePartData)
    {
        await SendToHotelAsync(hotelId, "LOW_STOCK", sparePartData);
    }
}
