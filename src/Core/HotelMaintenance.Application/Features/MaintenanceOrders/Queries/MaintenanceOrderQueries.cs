using HotelMaintenance.Application.Common;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using MediatR;

namespace HotelMaintenance.Application.Features.MaintenanceOrders.Queries;

// Get Order by ID Query
public record GetMaintenanceOrderByIdQuery(long OrderId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Get Orders with Filters Query
public record GetMaintenanceOrdersQuery(MaintenanceOrderFilterDto FilterDto) 
    : IRequest<Result<PagedResult<MaintenanceOrderDto>>>;

// Get Orders by Hotel Query
public record GetOrdersByHotelQuery(int HotelId, int PageNumber = 1, int PageSize = 20) 
    : IRequest<Result<PagedResult<MaintenanceOrderDto>>>;

// Get Orders by Department Query
public record GetOrdersByDepartmentQuery(int DepartmentId, int PageNumber = 1, int PageSize = 20) 
    : IRequest<Result<PagedResult<MaintenanceOrderDto>>>;

// Get Assigned Orders Query
public record GetAssignedOrdersQuery(int UserId, int PageNumber = 1, int PageSize = 20) 
    : IRequest<Result<PagedResult<MaintenanceOrderDto>>>;

// Get My Orders Query (created by user)
public record GetMyOrdersQuery(int UserId, int PageNumber = 1, int PageSize = 20) 
    : IRequest<Result<PagedResult<MaintenanceOrderDto>>>;

// Get Overdue Orders Query
public record GetOverdueOrdersQuery(int? HotelId = null) 
    : IRequest<Result<IEnumerable<MaintenanceOrderDto>>>;

// Get SLA Breached Orders Query
public record GetSLABreachedOrdersQuery(int? HotelId = null) 
    : IRequest<Result<IEnumerable<MaintenanceOrderDto>>>;

// Get Order Status History Query
public record GetOrderStatusHistoryQuery(long OrderId) 
    : IRequest<Result<IEnumerable<OrderStatusHistoryDto>>>;

// Get Order Assignment History Query
public record GetOrderAssignmentHistoryQuery(long OrderId) 
    : IRequest<Result<IEnumerable<OrderAssignmentHistoryDto>>>;

// Get Order Comments Query
public record GetOrderCommentsQuery(long OrderId) 
    : IRequest<Result<IEnumerable<OrderCommentDto>>>;

// Get Order Attachments Query
public record GetOrderAttachmentsQuery(long OrderId) 
    : IRequest<Result<IEnumerable<OrderAttachmentDto>>>;
