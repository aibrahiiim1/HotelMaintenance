using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using HotelMaintenance.Domain.Enums;
using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMaintenance.API.Controllers;

/// <summary>
/// Maintenance Orders management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MaintenanceOrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<MaintenanceOrdersController> _logger;

    public MaintenanceOrdersController(
        IUnitOfWork unitOfWork,
        JwtTokenService jwtTokenService,
        ILogger<MaintenanceOrdersController> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Get all maintenance orders with optional filtering
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Orders.View")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceOrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? hotelId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] OrderPriority? priority = null,
        [FromQuery] int? assignedToUserId = null)
    {
        try
        {
            IEnumerable<Domain.Entities.MaintenanceOrder> orders;

            if (hotelId.HasValue)
            {
                orders = await _unitOfWork.MaintenanceOrders.GetOrdersByHotelAsync(hotelId.Value);
            }
            else if (departmentId.HasValue)
            {
                orders = await _unitOfWork.MaintenanceOrders.GetOrdersByDepartmentAsync(departmentId.Value);
            }
            else if (status.HasValue)
            {
                orders = await _unitOfWork.MaintenanceOrders.GetOrdersByStatusAsync(status.Value);
            }
            else if (assignedToUserId.HasValue)
            {
                orders = await _unitOfWork.MaintenanceOrders.GetAssignedOrdersAsync(assignedToUserId.Value);
            }
            else
            {
                orders = await _unitOfWork.MaintenanceOrders.GetAllAsync();
            }

            // Apply additional filters
            if (priority.HasValue)
            {
                orders = orders.Where(o => o.Priority == priority.Value);
            }

            var result = orders.Select(o => new MaintenanceOrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Title = o.Title,
                Priority = o.Priority,
                CurrentStatus = o.CurrentStatus,
                Type = o.Type,
                HotelName = o.Hotel.Name,
                LocationName = o.Location?.Name,
                AssignedToUserName = o.AssignedToUser?.FullName,
                CreatedAt = o.CreatedAt,
                ExpectedCompletionDate = o.ExpectedCompletionDate,
                IsSLABreached = o.IsSLABreached
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting maintenance orders");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get maintenance order by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "Orders.View")]
    [ProducesResponseType(typeof(MaintenanceOrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            var result = new MaintenanceOrderDetailDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Title = order.Title,
                Description = order.Description,
                Priority = order.Priority,
                Type = order.Type,
                CurrentStatus = order.CurrentStatus,
                AssignmentStatus = order.AssignmentStatus,
                HotelId = order.HotelId,
                HotelName = order.Hotel.Name,
                RequestingDepartmentId = order.RequestingDepartmentId,
                RequestingDepartmentName = order.RequestingDepartment.Name,
                AssignedDepartmentId = order.AssignedDepartmentId,
                AssignedDepartmentName = order.AssignedDepartment?.Name,
                LocationId = order.LocationId,
                LocationName = order.Location?.Name,
                ItemId = order.ItemId,
                ItemName = order.Item?.Name,
                AssignedToUserId = order.AssignedToUserId,
                AssignedToUserName = order.AssignedToUser?.FullName,
                CreatedByUserId = order.CreatedByUserId,
                CreatedByUserName = order.CreatedByUser.FullName,
                EstimatedCost = order.EstimatedCost,
                ActualCost = order.ActualCost,
                ExpectedCompletionDate = order.ExpectedCompletionDate,
                ActualCompletionDate = order.ActualCompletionDate,
                CreatedAt = order.CreatedAt,
                IsSLABreached = order.IsSLABreached,
                StatusHistory = order.StatusHistory.Select(h => new OrderStatusHistoryDto
                {
                    Id = h.Id,
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByUserName = h.ChangedByUser.FullName,
                    Notes = h.Notes
                }).ToList(),
                Comments = order.Comments.Select(c => new OrderCommentDto
                {
                    Id = c.Id,
                    CommentText = c.CommentText,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User.FullName,
                    IsInternal = c.IsInternal
                }).ToList(),
                Attachments = order.Attachments.Select(a => new OrderAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    FileType = a.FileType,
                    UploadedAt = a.UploadedAt,
                    UploadedByUserName = a.UploadedByUser.FullName
                }).ToList()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Create new maintenance order
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Orders.Create")]
    [ProducesResponseType(typeof(MaintenanceOrderDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceOrderDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _jwtTokenService.GetUserIdFromClaims(User);

            await _unitOfWork.BeginTransactionAsync();

            // Generate order number
            var orderNumber = await _unitOfWork.MaintenanceOrders.GenerateOrderNumberAsync(dto.HotelId);

            // Create order
            var order = new Domain.Entities.MaintenanceOrder
            {
                OrderNumber = orderNumber,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Type = dto.Type,
                CurrentStatus = OrderStatus.Draft,
                AssignmentStatus = AssignmentStatus.NotAssigned,
                HotelId = dto.HotelId,
                RequestingDepartmentId = dto.RequestingDepartmentId,
                LocationId = dto.LocationId,
                ItemId = dto.ItemId,
                CreatedByUserId = userId,
                ExpectedCompletionDate = dto.ExpectedCompletionDate,
                EstimatedCost = dto.EstimatedCost,
                GuestName = dto.GuestName,
                GuestRoomNumber = dto.GuestRoomNumber,
                InternalNotes = dto.InternalNotes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MaintenanceOrders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Add initial status history
            var statusHistory = new Domain.Entities.OrderStatusHistory
            {
                MaintenanceOrderId = order.Id,
                FromStatus = OrderStatus.Draft,
                ToStatus = OrderStatus.Draft,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = userId,
                Notes = "Order created"
            };
            await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderNumber} created by user {UserId}", orderNumber, userId);

            // Return created order
            var createdOrder = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, createdOrder);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating maintenance order");
            return StatusCode(500, new { message = "An error occurred while creating the order" });
        }
    }

    /// <summary>
    /// Update maintenance order
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "Orders.Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMaintenanceOrderDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            // Update fields
            order.Title = dto.Title;
            order.Description = dto.Description;
            order.Priority = dto.Priority;
            order.Type = dto.Type;
            order.LocationId = dto.LocationId;
            order.ItemId = dto.ItemId;
            order.ExpectedCompletionDate = dto.ExpectedCompletionDate;
            order.EstimatedCost = dto.EstimatedCost;
            order.InternalNotes = dto.InternalNotes;
            order.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} updated", id);

            return Ok(new { message = "Order updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Assign maintenance order to user
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Policy = "Orders.Assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignOrder(long id, [FromBody] AssignOrderDto dto)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            var userId = _jwtTokenService.GetUserIdFromClaims(User);

            await _unitOfWork.BeginTransactionAsync();

            var previousUserId = order.AssignedToUserId;

            // Update assignment
            order.AssignedToUserId = dto.AssignedToUserId;
            order.AssignedDepartmentId = dto.AssignedDepartmentId;
            order.AssignmentStatus = AssignmentStatus.Assigned;
            order.AssignedAt = DateTime.UtcNow;
            order.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.MaintenanceOrders.Update(order);

            // Add assignment history
            var assignmentHistory = new Domain.Entities.OrderAssignmentHistory
            {
                MaintenanceOrderId = id,
                FromUserId = previousUserId,
                ToUserId = dto.AssignedToUserId,
                FromDepartmentId = order.AssignedDepartmentId,
                ToDepartmentId = dto.AssignedDepartmentId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = userId,
                Notes = dto.Notes
            };
            await _unitOfWork.OrderAssignmentHistory.AddAsync(assignmentHistory);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderId} assigned to user {UserId}", id, dto.AssignedToUserId);

            return Ok(new { message = "Order assigned successfully" });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error assigning order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Change order status
    /// </summary>
    [HttpPost("{id}/status")]
    [Authorize(Policy = "Orders.Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeOrderStatusDto dto)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            var userId = _jwtTokenService.GetUserIdFromClaims(User);

            await _unitOfWork.BeginTransactionAsync();

            var previousStatus = order.CurrentStatus;

            // Update status
            order.CurrentStatus = dto.NewStatus;
            order.LastModifiedAt = DateTime.UtcNow;

            // Set completion date if completed
            if (dto.NewStatus == OrderStatus.Completed && order.ActualCompletionDate == null)
            {
                order.ActualCompletionDate = DateTime.UtcNow;
            }

            _unitOfWork.MaintenanceOrders.Update(order);

            // Add status history
            var statusHistory = new Domain.Entities.OrderStatusHistory
            {
                MaintenanceOrderId = id,
                FromStatus = previousStatus,
                ToStatus = dto.NewStatus,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = userId,
                Notes = dto.Notes
            };
            await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderId} status changed from {FromStatus} to {ToStatus}", 
                id, previousStatus, dto.NewStatus);

            return Ok(new { message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error changing status for order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Add comment to order
    /// </summary>
    [HttpPost("{id}/comments")]
    [Authorize(Policy = "Orders.View")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(long id, [FromBody] AddOrderCommentDto dto)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            var userId = _jwtTokenService.GetUserIdFromClaims(User);

            var comment = new Domain.Entities.OrderComment
            {
                MaintenanceOrderId = id,
                UserId = userId,
                CommentText = dto.CommentText,
                IsInternal = dto.IsInternal,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OrderComments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comment added to order {OrderId} by user {UserId}", id, userId);

            return CreatedAtAction(nameof(GetById), new { id }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get overdue orders
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "Orders.View")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceOrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueOrders()
    {
        try
        {
            var orders = await _unitOfWork.MaintenanceOrders.GetOverdueOrdersAsync();
            
            var result = orders.Select(o => new MaintenanceOrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Title = o.Title,
                Priority = o.Priority,
                CurrentStatus = o.CurrentStatus,
                HotelName = o.Hotel.Name,
                AssignedToUserName = o.AssignedToUser?.FullName,
                ExpectedCompletionDate = o.ExpectedCompletionDate,
                CreatedAt = o.CreatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue orders");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get SLA breached orders
    /// </summary>
    [HttpGet("sla-breached")]
    [Authorize(Policy = "Orders.View")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceOrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSLABreachedOrders()
    {
        try
        {
            var orders = await _unitOfWork.MaintenanceOrders.GetSLABreachedOrdersAsync();
            
            var result = orders.Select(o => new MaintenanceOrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Title = o.Title,
                Priority = o.Priority,
                CurrentStatus = o.CurrentStatus,
                HotelName = o.Hotel.Name,
                AssignedToUserName = o.AssignedToUser?.FullName,
                IsSLABreached = o.IsSLABreached,
                CreatedAt = o.CreatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting SLA breached orders");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get my assigned orders
    /// </summary>
    [HttpGet("my-orders")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceOrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            var userId = _jwtTokenService.GetUserIdFromClaims(User);
            var orders = await _unitOfWork.MaintenanceOrders.GetAssignedOrdersAsync(userId);
            
            var result = orders.Select(o => new MaintenanceOrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Title = o.Title,
                Priority = o.Priority,
                CurrentStatus = o.CurrentStatus,
                Type = o.Type,
                HotelName = o.Hotel.Name,
                LocationName = o.Location?.Name,
                ExpectedCompletionDate = o.ExpectedCompletionDate,
                CreatedAt = o.CreatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user orders");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Delete/Cancel order
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Orders.Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, [FromQuery] string? reason = null)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            // Soft delete by setting as cancelled
            order.IsCancelled = true;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason;
            order.CurrentStatus = OrderStatus.Cancelled;

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} cancelled", id);

            return Ok(new { message = "Order cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}
