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
