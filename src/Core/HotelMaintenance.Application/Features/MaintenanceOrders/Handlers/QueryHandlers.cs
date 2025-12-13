using AutoMapper;
using HotelMaintenance.Application.Common;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using HotelMaintenance.Application.Features.MaintenanceOrders.Queries;
using HotelMaintenance.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;
using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Application.Features.MaintenanceOrders.Handlers;

public class GetMaintenanceOrderByIdHandler : IRequestHandler<GetMaintenanceOrderByIdQuery, Result<MaintenanceOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaintenanceOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MaintenanceOrderDto>> Handle(GetMaintenanceOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _unitOfWork.MaintenanceOrders.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
            
            if (order == null)
                return Result<MaintenanceOrderDto>.Failure("Order not found");

            var orderDto = _mapper.Map<MaintenanceOrderDto>(order);
            return Result<MaintenanceOrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            return Result<MaintenanceOrderDto>.Failure($"Error retrieving order: {ex.Message}");
        }
    }
}

public class GetMaintenanceOrdersHandler : IRequestHandler<GetMaintenanceOrdersQuery, Result<PagedResult<MaintenanceOrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaintenanceOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<MaintenanceOrderDto>>> Handle(GetMaintenanceOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter predicate
            Expression<Func<MaintenanceOrder, bool>> predicate = order => true;

            if (request.FilterDto.HotelId.HasValue)
                predicate = CombinePredicates(predicate, o => o.HotelId == request.FilterDto.HotelId.Value);

            if (request.FilterDto.DepartmentId.HasValue)
                predicate = CombinePredicates(predicate, o => o.RequestingDepartmentId == request.FilterDto.DepartmentId.Value 
                    || o.AssignedDepartmentId == request.FilterDto.DepartmentId.Value);

            if (request.FilterDto.AssignedToUserId.HasValue)
                predicate = CombinePredicates(predicate, o => o.AssignedToUserId == request.FilterDto.AssignedToUserId.Value);

            if (request.FilterDto.LocationId.HasValue)
                predicate = CombinePredicates(predicate, o => o.LocationId == request.FilterDto.LocationId.Value);

            if (request.FilterDto.ItemId.HasValue)
                predicate = CombinePredicates(predicate, o => o.ItemId == request.FilterDto.ItemId.Value);

            if (request.FilterDto.Status.HasValue)
                predicate = CombinePredicates(predicate, o => o.CurrentStatus == request.FilterDto.Status.Value);

            if (request.FilterDto.Priority.HasValue)
                predicate = CombinePredicates(predicate, o => o.Priority == request.FilterDto.Priority.Value);

            if (request.FilterDto.Type.HasValue)
                predicate = CombinePredicates(predicate, o => o.Type == request.FilterDto.Type.Value);

            if (request.FilterDto.AssignmentStatus.HasValue)
                predicate = CombinePredicates(predicate, o => o.AssignmentStatus == request.FilterDto.AssignmentStatus.Value);

            if (request.FilterDto.IsUrgent.HasValue)
                predicate = CombinePredicates(predicate, o => o.IsUrgent == request.FilterDto.IsUrgent.Value);

            if (request.FilterDto.IsSafetyRelated.HasValue)
                predicate = CombinePredicates(predicate, o => o.IsSafetyRelated == request.FilterDto.IsSafetyRelated.Value);

            if (request.FilterDto.IsSLABreached.HasValue)
                predicate = CombinePredicates(predicate, o => o.IsSLABreached == request.FilterDto.IsSLABreached.Value);

            if (request.FilterDto.IsOverdue.HasValue && request.FilterDto.IsOverdue.Value)
                predicate = CombinePredicates(predicate, o => o.ExpectedCompletionDate < DateTime.UtcNow 
                    && o.CurrentStatus != OrderStatus.Completed && o.CurrentStatus != OrderStatus.Closed);

            if (request.FilterDto.CreatedFrom.HasValue)
                predicate = CombinePredicates(predicate, o => o.CreatedAt >= request.FilterDto.CreatedFrom.Value);

            if (request.FilterDto.CreatedTo.HasValue)
                predicate = CombinePredicates(predicate, o => o.CreatedAt <= request.FilterDto.CreatedTo.Value);

            if (!string.IsNullOrWhiteSpace(request.FilterDto.SearchTerm))
            {
                var searchTerm = request.FilterDto.SearchTerm.ToLower();
                predicate = CombinePredicates(predicate, o => 
                    o.OrderNumber.ToLower().Contains(searchTerm) ||
                    o.Title.ToLower().Contains(searchTerm) ||
                    o.Description.ToLower().Contains(searchTerm));
            }

            // Get filtered orders
            var allOrders = await _unitOfWork.MaintenanceOrders.FindAsync(predicate, cancellationToken);
            var ordersQuery = allOrders.AsQueryable();

            // Apply sorting
            ordersQuery = request.FilterDto.SortBy?.ToLower() switch
            {
                "priority" => request.FilterDto.SortOrder == "asc" 
                    ? ordersQuery.OrderBy(o => o.Priority) 
                    : ordersQuery.OrderByDescending(o => o.Priority),
                "status" => request.FilterDto.SortOrder == "asc" 
                    ? ordersQuery.OrderBy(o => o.CurrentStatus) 
                    : ordersQuery.OrderByDescending(o => o.CurrentStatus),
                "expectedcompletiondate" => request.FilterDto.SortOrder == "asc" 
                    ? ordersQuery.OrderBy(o => o.ExpectedCompletionDate) 
                    : ordersQuery.OrderByDescending(o => o.ExpectedCompletionDate),
                _ => request.FilterDto.SortOrder == "asc" 
                    ? ordersQuery.OrderBy(o => o.CreatedAt) 
                    : ordersQuery.OrderByDescending(o => o.CreatedAt)
            };

            // Get total count
            var totalCount = ordersQuery.Count();

            // Apply pagination
            var orders = ordersQuery
                .Skip((request.FilterDto.PageNumber - 1) * request.FilterDto.PageSize)
                .Take(request.FilterDto.PageSize)
                .ToList();

            // Map to DTOs
            var orderDtos = _mapper.Map<List<MaintenanceOrderDto>>(orders);

            var pagedResult = new PagedResult<MaintenanceOrderDto>
            {
                Items = orderDtos,
                TotalCount = totalCount,
                PageNumber = request.FilterDto.PageNumber,
                PageSize = request.FilterDto.PageSize
            };

            return Result<PagedResult<MaintenanceOrderDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<MaintenanceOrderDto>>.Failure($"Error retrieving orders: {ex.Message}");
        }
    }

    private Expression<Func<T, bool>> CombinePredicates<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}

public class GetOrderStatusHistoryHandler : IRequestHandler<GetOrderStatusHistoryQuery, Result<IEnumerable<OrderStatusHistoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderStatusHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<OrderStatusHistoryDto>>> Handle(GetOrderStatusHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var history = await _unitOfWork.OrderStatusHistory.FindAsync(
                h => h.MaintenanceOrderId == request.OrderId, 
                cancellationToken);

            var historyDtos = _mapper.Map<List<OrderStatusHistoryDto>>(history.OrderByDescending(h => h.ChangedAt));
            return Result<IEnumerable<OrderStatusHistoryDto>>.Success(historyDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderStatusHistoryDto>>.Failure($"Error retrieving status history: {ex.Message}");
        }
    }
}

public class GetOrderCommentsHandler : IRequestHandler<GetOrderCommentsQuery, Result<IEnumerable<OrderCommentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderCommentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<OrderCommentDto>>> Handle(GetOrderCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _unitOfWork.OrderComments.FindAsync(
                c => c.MaintenanceOrderId == request.OrderId, 
                cancellationToken);

            var commentDtos = _mapper.Map<List<OrderCommentDto>>(comments.OrderBy(c => c.CreatedAt));
            return Result<IEnumerable<OrderCommentDto>>.Success(commentDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderCommentDto>>.Failure($"Error retrieving comments: {ex.Message}");
        }
    }
}
