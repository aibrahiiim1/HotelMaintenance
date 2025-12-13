using AutoMapper;
using FluentValidation;
using HotelMaintenance.Application.Common;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using HotelMaintenance.Application.Features.MaintenanceOrders.Commands;
using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;
using HotelMaintenance.Domain.Exceptions;
using HotelMaintenance.Domain.Interfaces;
using MediatR;

namespace HotelMaintenance.Application.Features.MaintenanceOrders.Handlers;

public class CreateMaintenanceOrderHandler : IRequestHandler<CreateMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private  IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMaintenanceOrderDto> _validator;

    public CreateMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateMaintenanceOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(CreateMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.OrderDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.OrderDto.HotelId, cancellationToken);
            if (hotel == null)
                return Result<MaintenanceOrderDto>.Failure("Hotel not found");

            // Check department exists
            var department = await _unitOfWork.Departments.GetByIdAsync(request.OrderDto.RequestingDepartmentId, cancellationToken);
            if (department == null)
                return Result<MaintenanceOrderDto>.Failure("Department not found");

            // Check location exists
            var location = await _unitOfWork.Locations.GetByIdAsync(request.OrderDto.LocationId, cancellationToken);
            if (location == null)
                return Result<MaintenanceOrderDto>.Failure("Location not found");

            // Check item if provided
            if (request.OrderDto.ItemId.HasValue)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(request.OrderDto.ItemId.Value, cancellationToken);
                if (item == null)
                    return Result<MaintenanceOrderDto>.Failure("Item not found");
            }

            // Generate order number
            var orderNumber = await _unitOfWork.MaintenanceOrders.GenerateOrderNumberAsync(request.OrderDto.HotelId, cancellationToken);

            // Create order entity
            var order = _mapper.Map<MaintenanceOrder>(request.OrderDto);
            order.OrderNumber = orderNumber;
            order.CreatedByUserId = request.CreatedByUserId;
            order.CreatedAt = DateTime.UtcNow;
            order.LastModifiedAt = DateTime.UtcNow;
            order.CurrentStatus = OrderStatus.Draft;
            order.AssignmentStatus = AssignmentStatus.NotAssigned;

            // Calculate SLA deadline based on priority
            var slaConfig = await _unitOfWork.SLAConfigurations.GetByHotelAndPriorityAsync(
                request.OrderDto.HotelId, 
                request.OrderDto.Priority, 
                cancellationToken);
            
            if (slaConfig != null)
            {
                order.SLADeadline = DateTime.UtcNow.AddMinutes(slaConfig.ResolutionTimeMinutes);
            }

            // Add to database
            await _unitOfWork.MaintenanceOrders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load related entities for DTO mapping
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);

            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);
            return Result<MaintenanceOrderDto>.Success(orderDto, "Order created successfully");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error creating order: {ex.Message}");
        }
    }
}

public class AssignMaintenanceOrderHandler : IRequestHandler<AssignMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AssignOrderDto> _validator;

    public AssignMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AssignOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(AssignMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.AssignDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if order can be assigned
            if (order.CurrentStatus == OrderStatus.Closed || order.CurrentStatus == OrderStatus.Cancelled)
                return Result<MaintenanceOrderDto>.Failure("Cannot assign a closed or cancelled order");

            // Check department exists
            var department = await _unitOfWork.Departments.GetByIdAsync(request.AssignDto.AssignedDepartmentId, cancellationToken);
            if (department == null)
                return Result<MaintenanceOrderDto>.Failure("Department not found");

            // Check user if provided
            if (request.AssignDto.AssignedToUserId.HasValue)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.AssignDto.AssignedToUserId.Value, cancellationToken);
                if (user == null)
                    return Result<MaintenanceOrderDto>.Failure("User not found");
                
                if (!user.IsAvailable)
                    return Result<MaintenanceOrderDto>.Failure("User is not available for assignment");
            }

            // Create assignment history
            var assignmentHistory = new OrderAssignmentHistory
            {
                MaintenanceOrderId = order.Id,
                FromDepartmentId = order.AssignedDepartmentId,
                ToDepartmentId = request.AssignDto.AssignedDepartmentId,
                FromUserId = order.AssignedToUserId,
                ToUserId = request.AssignDto.AssignedToUserId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = request.AssignedByUserId,
                Reason = request.AssignDto.Reason
            };
            await _unitOfWork.OrderAssignmentHistory.AddAsync(assignmentHistory, cancellationToken);

            // Update order
            order.AssignedDepartmentId = request.AssignDto.AssignedDepartmentId;
            order.AssignedToUserId = request.AssignDto.AssignedToUserId;
            order.AssignedAt = DateTime.UtcNow;
            order.AssignedByUserId = request.AssignedByUserId;
            order.AssignmentStatus = AssignmentStatus.Assigned;
            order.LastModifiedAt = DateTime.UtcNow;

            // Update status if currently Draft or Submitted
            if (order.CurrentStatus == OrderStatus.Draft || order.CurrentStatus == OrderStatus.Submitted)
            {
                var oldStatus = order.CurrentStatus;
                order.CurrentStatus = OrderStatus.Assigned;

                // Add status history
                var statusHistory = new OrderStatusHistory
                {
                    MaintenanceOrderId = order.Id,
                    FromStatus = oldStatus,
                    ToStatus = OrderStatus.Assigned,
                    ChangedAt = DateTime.UtcNow,
                    ChangedByUserId = request.AssignedByUserId,
                    Notes = "Order assigned"
                };
                await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);
            }

            // Calculate response time
            if (order.SubmittedAt.HasValue)
            {
                order.ResponseTimeMinutes = (int)(DateTime.UtcNow - order.SubmittedAt.Value).TotalMinutes;
            }

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);
            
            return Result<MaintenanceOrderDto>.Success(orderDto, "Order assigned successfully");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error assigning order: {ex.Message}");
        }
    }
}

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateOrderStatusDto> _validator;

    public UpdateOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateOrderStatusDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.StatusDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if status transition is valid
            if (!IsValidStatusTransition(order.CurrentStatus, request.StatusDto.NewStatus))
                return Result<MaintenanceOrderDto>.Failure($"Cannot transition from {order.CurrentStatus} to {request.StatusDto.NewStatus}");

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                MaintenanceOrderId = order.Id,
                FromStatus = order.CurrentStatus,
                ToStatus = request.StatusDto.NewStatus,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = request.ChangedByUserId,
                Notes = request.StatusDto.Notes
            };
            await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);

            // Update order
            order.CurrentStatus = request.StatusDto.NewStatus;
            order.LastModifiedAt = DateTime.UtcNow;
            order.LastModifiedByUserId = request.ChangedByUserId;

            // Handle specific status updates
            switch (request.StatusDto.NewStatus)
            {
                case OrderStatus.Submitted:
                    order.SubmittedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.InProgress:
                    order.ActualStartDate = request.StatusDto.ActualStartDate ?? DateTime.UtcNow;
                    break;
                case OrderStatus.Scheduled:
                    order.ScheduledStartDate = request.StatusDto.ScheduledStartDate;
                    break;
                case OrderStatus.Completed:
                    order.ActualCompletionDate = request.StatusDto.ActualCompletionDate ?? DateTime.UtcNow;
                    order.CompletedByUserId = request.ChangedByUserId;
                    order.ResolutionNotes = request.StatusDto.ResolutionNotes;
                    
                    // Calculate resolution time
                    if (order.SubmittedAt.HasValue)
                    {
                        order.ResolutionTimeMinutes = (int)(DateTime.UtcNow - order.SubmittedAt.Value).TotalMinutes;
                    }
                    break;
            }

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);
            
            return Result<MaintenanceOrderDto>.Success(orderDto, "Order status updated successfully");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error updating order status: {ex.Message}");
        }
    }

    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // Define valid status transitions
        var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
        {
            { OrderStatus.Draft, new() { OrderStatus.Submitted, OrderStatus.Cancelled } },
            { OrderStatus.Submitted, new() { OrderStatus.Assigned, OrderStatus.Scheduled, OrderStatus.Cancelled } },
            { OrderStatus.Assigned, new() { OrderStatus.InProgress, OrderStatus.Scheduled, OrderStatus.ExternalWork, OrderStatus.Cancelled } },
            { OrderStatus.InProgress, new() { OrderStatus.OnHold, OrderStatus.AwaitingParts, OrderStatus.ExternalWork, OrderStatus.Completed, OrderStatus.Rejected } },
            { OrderStatus.OnHold, new() { OrderStatus.InProgress, OrderStatus.Cancelled } },
            { OrderStatus.AwaitingParts, new() { OrderStatus.InProgress, OrderStatus.Cancelled } },
            { OrderStatus.ExternalWork, new() { OrderStatus.InProgress, OrderStatus.Completed, OrderStatus.Cancelled } },
            { OrderStatus.Scheduled, new() { OrderStatus.Assigned, OrderStatus.InProgress, OrderStatus.Cancelled } },
            { OrderStatus.Completed, new() { OrderStatus.Verified, OrderStatus.Reopened, OrderStatus.Closed } },
            { OrderStatus.Verified, new() { OrderStatus.Closed, OrderStatus.Reopened } },
            { OrderStatus.Reopened, new() { OrderStatus.InProgress, OrderStatus.Completed } }
        };

        return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
    }
}

/// <summary>
/// UPDATE ORDER HANDLER - Updates maintenance order details
/// </summary>
public class UpdateMaintenanceOrderHandler : IRequestHandler<UpdateMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateMaintenanceOrderDto> _validator;

    public UpdateMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateMaintenanceOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(UpdateMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.OrderDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if order can be updated
            if (order.CurrentStatus == OrderStatus.Completed || order.CurrentStatus == OrderStatus.Closed)
                return Result<MaintenanceOrderDto>.Failure("Cannot update a completed or closed order");

            // Update order fields
            order.Title = request.OrderDto.Title;
            order.Description = request.OrderDto.Description;
            order.Priority = request.OrderDto.Priority;
            order.LocationId = request.OrderDto.LocationId;
            order.ItemId = request.OrderDto.ItemId;
            order.ExpectedCompletionDate = request.OrderDto.ExpectedCompletionDate;
            order.IsUrgent = request.OrderDto.IsUrgent;
            order.IsSafetyRelated = request.OrderDto.IsSafetyRelated;
            order.LastModifiedAt = DateTime.UtcNow;
            order.LastModifiedByUserId = request.ModifiedByUserId;

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);

            return Result<MaintenanceOrderDto>.Success(orderDto, "Order updated successfully");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error updating order: {ex.Message}");
        }
    }
}

/// <summary>
/// COMPLETE ORDER HANDLER - Marks order as completed with resolution details
/// </summary>
public class CompleteMaintenanceOrderHandler : IRequestHandler<CompleteMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CompleteOrderDto> _validator;

    public CompleteMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CompleteOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(CompleteMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.CompleteDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if order can be completed
            if (order.CurrentStatus == OrderStatus.Completed || order.CurrentStatus == OrderStatus.Closed)
                return Result<MaintenanceOrderDto>.Failure("Order is already completed or closed");

            if (order.CurrentStatus == OrderStatus.Draft || order.CurrentStatus == OrderStatus.Submitted)
                return Result<MaintenanceOrderDto>.Failure("Cannot complete an order that hasn't been started");

            // Update order with completion details
            var previousStatus = order.CurrentStatus;
            order.CurrentStatus = OrderStatus.Completed;
            order.ActualCompletionDate = DateTime.UtcNow;
            order.CompletedByUserId = request.CompletedByUserId;
            order.ResolutionNotes = request.CompleteDto.ResolutionNotes;
            order.RequiresFollowUp = request.CompleteDto.RequiresFollowUp;
            order.FollowUpDate = request.CompleteDto.FollowUpDate;
            order.LaborCost = request.CompleteDto.LaborCost;
            order.MaterialCost = request.CompleteDto.MaterialCost;
            order.LastModifiedAt = DateTime.UtcNow;
            order.LastModifiedByUserId = request.CompletedByUserId;

            // Calculate resolution time
            if (order.SubmittedAt.HasValue)
            {
                order.ResolutionTimeMinutes = (int)(DateTime.UtcNow - order.SubmittedAt.Value).TotalMinutes;
            }

            // Add spare parts usage
            foreach (var sparePartUsage in request.CompleteDto.SparePartsUsed)
            {
                var orderSparePartUsage = new OrderSparePartUsage
                {
                    MaintenanceOrderId = order.Id,
                    SparePartId = sparePartUsage.SparePartId,
                    QuantityUsed = sparePartUsage.QuantityUsed,
                    UnitCost = sparePartUsage.UnitCost,
                    TotalCost = sparePartUsage.QuantityUsed * sparePartUsage.UnitCost,
                    UsedByUserId = request.CompletedByUserId,
                    UsedAt = DateTime.UtcNow,
                    Notes = sparePartUsage.Notes
                };
                await _unitOfWork.OrderSparePartUsage.AddAsync(orderSparePartUsage, cancellationToken);

                // Update spare part inventory
                var sparePart = await _unitOfWork.SpareParts.GetByIdAsync(sparePartUsage.SparePartId, cancellationToken);
                if (sparePart != null)
                {
                    var quantityBefore = sparePart.QuantityOnHand;
                    sparePart.QuantityOnHand -= sparePartUsage.QuantityUsed;
                    _unitOfWork.SpareParts.Update(sparePart);

                    // Add spare part transaction
                    var transaction = new SparePartTransaction
                    {
                        SparePartId = sparePartUsage.SparePartId,
                        Type = TransactionType.Usage,
                        Quantity = -sparePartUsage.QuantityUsed,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = sparePart.QuantityOnHand,
                        UnitCost = sparePartUsage.UnitCost,
                        TotalCost = sparePartUsage.QuantityUsed * sparePartUsage.UnitCost,
                        ReferenceId = order.Id,
                        ReferenceType = "MaintenanceOrder",
                        ReferenceNumber = order.OrderNumber,
                        TransactionByUserId = request.CompletedByUserId,
                        TransactionDate = DateTime.UtcNow,
                        Notes = $"Used in order {order.OrderNumber}"
                    };
                    await _unitOfWork.SparePartTransactions.AddAsync(transaction, cancellationToken);
                }
            }

            // Calculate total actual cost
            var sparePartsCost = request.CompleteDto.SparePartsUsed.Sum(sp => sp.QuantityUsed * sp.UnitCost);
            order.ActualCost = request.CompleteDto.LaborCost + request.CompleteDto.MaterialCost + sparePartsCost;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                MaintenanceOrderId = order.Id,
                FromStatus = previousStatus,
                ToStatus = OrderStatus.Completed,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = request.CompletedByUserId,
                Notes = "Order completed"
            };
            await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);

            return Result<MaintenanceOrderDto>.Success(orderDto, "Order completed successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<MaintenanceOrderDto>.Failure($"Error completing order: {ex.Message}");
        }
    }
}

/// <summary>
/// VERIFY ORDER HANDLER - Requester verifies completed work
/// </summary>
public class VerifyMaintenanceOrderHandler : IRequestHandler<VerifyMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<VerifyOrderDto> _validator;

    public VerifyMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<VerifyOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(VerifyMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.VerifyDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if order can be verified
            if (order.CurrentStatus != OrderStatus.Completed)
                return Result<MaintenanceOrderDto>.Failure("Only completed orders can be verified");

            // Only requester can verify
            if (order.CreatedByUserId != request.VerifiedByUserId)
                return Result<MaintenanceOrderDto>.Failure("Only the order creator can verify the work");

            // Update order with verification
            var previousStatus = order.CurrentStatus;
            order.IsApprovedByRequester = request.VerifyDto.IsApproved;
            order.ApprovedByUserId = request.VerifiedByUserId;
            order.ApprovedAt = DateTime.UtcNow;
            order.Rating = request.VerifyDto.Rating;
            order.RequesterFeedback = request.VerifyDto.Feedback;
            order.LastModifiedAt = DateTime.UtcNow;

            if (request.VerifyDto.IsApproved)
            {
                order.CurrentStatus = OrderStatus.Verified;

                // Add status history
                var statusHistory = new OrderStatusHistory
                {
                    MaintenanceOrderId = order.Id,
                    FromStatus = previousStatus,
                    ToStatus = OrderStatus.Verified,
                    ChangedAt = DateTime.UtcNow,
                    ChangedByUserId = request.VerifiedByUserId,
                    Notes = $"Work verified with rating: {request.VerifyDto.Rating}/5"
                };
                await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);
            }
            else
            {
                // If not approved, reopen the order
                order.CurrentStatus = OrderStatus.Reopened;
                order.IsRejected = true;
                order.RejectedAt = DateTime.UtcNow;
                order.RejectedByUserId = request.VerifiedByUserId;
                order.RejectionReason = request.VerifyDto.Feedback;

                var statusHistory = new OrderStatusHistory
                {
                    MaintenanceOrderId = order.Id,
                    FromStatus = previousStatus,
                    ToStatus = OrderStatus.Reopened,
                    ChangedAt = DateTime.UtcNow,
                    ChangedByUserId = request.VerifiedByUserId,
                    Notes = "Work rejected by requester"
                };
                await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);
            }

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);

            return Result<MaintenanceOrderDto>.Success(orderDto,
                request.VerifyDto.IsApproved ? "Order verified successfully" : "Order reopened for rework");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error verifying order: {ex.Message}");
        }
    }
}

/// <summary>
/// CANCEL ORDER HANDLER - Cancels maintenance order
/// </summary>
public class CancelMaintenanceOrderHandler : IRequestHandler<CancelMaintenanceOrderCommand, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CancelOrderDto> _validator;

    public CancelMaintenanceOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CancelOrderDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(CancelMaintenanceOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.CancelDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MaintenanceOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Get order
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            // Check if order can be cancelled
            if (order.CurrentStatus == OrderStatus.Completed || order.CurrentStatus == OrderStatus.Closed)
                return Result<MaintenanceOrderDto>.Failure("Cannot cancel a completed or closed order");

            if (order.IsCancelled)
                return Result<MaintenanceOrderDto>.Failure("Order is already cancelled");

            // Cancel order
            var previousStatus = order.CurrentStatus;
            order.CurrentStatus = OrderStatus.Cancelled;
            order.IsCancelled = true;
            order.CancelledAt = DateTime.UtcNow;
            order.CancelledByUserId = request.CancelledByUserId;
            order.CancellationReason = request.CancelDto.CancellationReason;
            order.LastModifiedAt = DateTime.UtcNow;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                MaintenanceOrderId = order.Id,
                FromStatus = previousStatus,
                ToStatus = OrderStatus.Cancelled,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = request.CancelledByUserId,
                Notes = $"Order cancelled: {request.CancelDto.CancellationReason}"
            };
            await _unitOfWork.OrderStatusHistory.AddAsync(statusHistory, cancellationToken);

            _unitOfWork.MaintenanceOrders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with details
            order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);

            return Result<MaintenanceOrderDto>.Success(orderDto, "Order cancelled successfully");
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error cancelling order: {ex.Message}");
        }
    }
}

/// <summary>
/// ADD COMMENT HANDLER - Adds comment to order
/// </summary>
public class AddOrderCommentHandler : IRequestHandler<AddOrderCommentCommand, Result<OrderCommentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderCommentDto> _validator;

    public AddOrderCommentHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateOrderCommentDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<OrderCommentDto>> Handle(AddOrderCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate
            var validationResult = await _validator.ValidateAsync(request.CommentDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<OrderCommentDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check if order exists
            var orderExists = await _unitOfWork.MaintenanceOrders.AnyAsync(
                o => o.Id == request.CommentDto.MaintenanceOrderId,
                cancellationToken);

            if (!orderExists)
                return Result<OrderCommentDto>.Failure("Order not found");

            // Create comment
            var comment = new OrderComment
            {
                MaintenanceOrderId = request.CommentDto.MaintenanceOrderId,
                UserId = request.UserId,
                Comment = request.CommentDto.Comment,
                IsInternal = request.CommentDto.IsInternal,
                CreatedAt = DateTime.UtcNow,
                IsEdited = false
            };

            await _unitOfWork.OrderComments.AddAsync(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load user info for DTO
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            var commentDto = new OrderCommentDto
            {
                Id = comment.Id,
                Comment = comment.Comment,
                UserId = comment.UserId,
                UserName = user?.FullName ?? "Unknown",
                IsInternal = comment.IsInternal,
                CreatedAt = comment.CreatedAt,
                IsEdited = comment.IsEdited
            };

            return Result<OrderCommentDto>.Success(commentDto, "Comment added successfully");
        }
        catch (Exception ex)
        {
            return Result<OrderCommentDto>.Failure($"Error adding comment: {ex.Message}");
        }
    }
}

/// <summary>
/// UPLOAD ATTACHMENT HANDLER - Uploads file attachment to order
/// </summary>
public class UploadOrderAttachmentHandler : IRequestHandler<UploadOrderAttachmentCommand, Result<OrderAttachmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UploadOrderAttachmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OrderAttachmentDto>> Handle(UploadOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if order exists
            var orderExists = await _unitOfWork.MaintenanceOrders.AnyAsync(
                o => o.Id == request.OrderId,
                cancellationToken);

            if (!orderExists)
                return Result<OrderAttachmentDto>.Failure("Order not found");

            // Create attachment record
            var attachment = new OrderAttachment
            {
                MaintenanceOrderId = request.OrderId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileSize = request.FileSize,
                FileType = request.FileType,
                Type = DetermineAttachmentType(request.FileType),
                Description = request.Description,
                UploadedByUserId = request.UploadedByUserId,
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.OrderAttachments.AddAsync(attachment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load user info
            var user = await _unitOfWork.Users.GetByIdAsync(request.UploadedByUserId, cancellationToken);

            var attachmentDto = new OrderAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                FileSize = attachment.FileSize,
                FileType = attachment.FileType,
                Type = attachment.Type,
                TypeName = attachment.Type.ToString(),
                Description = attachment.Description,
                UploadedByUserName = user?.FullName ?? "Unknown",
                UploadedAt = attachment.UploadedAt
            };

            return Result<OrderAttachmentDto>.Success(attachmentDto, "Attachment uploaded successfully");
        }
        catch (Exception ex)
        {
            return Result<OrderAttachmentDto>.Failure($"Error uploading attachment: {ex.Message}");
        }
    }

    private AttachmentType DetermineAttachmentType(string fileType)
    {
        return fileType.ToLower() switch
        {
            var t when t.Contains("image") => AttachmentType.Photo,
            var t when t.Contains("video") => AttachmentType.Video,
            var t when t.Contains("audio") => AttachmentType.Audio,
            var t when t.Contains("pdf") || t.Contains("doc") || t.Contains("xls") => AttachmentType.Document,
            _ => AttachmentType.Other
        };
    }
}
