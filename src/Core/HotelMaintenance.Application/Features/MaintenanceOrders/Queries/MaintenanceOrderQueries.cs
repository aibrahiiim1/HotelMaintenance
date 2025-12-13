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

///// <summary>
///// Query to get all orders for a specific hotel (paginated)
///// </summary>
//public class GetOrdersByHotelQuery : IRequest<Result<PagedResult<MaintenanceOrderDto>>>
//{
//    public int HotelId { get; set; }
//    public int PageNumber { get; set; } = 1;
//    public int PageSize { get; set; } = 10;
//}

///// <summary>
///// Query to get all orders for a specific department (paginated)
///// </summary>
//public class GetOrdersByDepartmentQuery : IRequest<Result<PagedResult<MaintenanceOrderDto>>>
//{
//    public int DepartmentId { get; set; }
//    public int PageNumber { get; set; } = 1;
//    public int PageSize { get; set; } = 10;
//}

///// <summary>
///// Query to get orders assigned to a specific user (paginated)
///// </summary>
//public class GetAssignedOrdersQuery : IRequest<Result<PagedResult<MaintenanceOrderDto>>>
//{
//    public string UserId { get; set; } = string.Empty;
//    public int PageNumber { get; set; } = 1;
//    public int PageSize { get; set; } = 10;
//}

///// <summary>
///// Query to get orders created by the current user (paginated)
///// </summary>
//public class GetMyOrdersQuery : IRequest<Result<PagedResult<MaintenanceOrderDto>>>
//{
//    public string UserId { get; set; } = string.Empty;
//    public int PageNumber { get; set; } = 1;
//    public int PageSize { get; set; } = 10;
//}

///// <summary>
///// Query to get all overdue orders
///// </summary>
//public class GetOverdueOrdersQuery : IRequest<Result<IEnumerable<MaintenanceOrderDto>>>
//{
//    public int? HotelId { get; set; }
//}

///// <summary>
///// Query to get all orders that have breached SLA
///// </summary>
//public class GetSLABreachedOrdersQuery : IRequest<Result<IEnumerable<MaintenanceOrderDto>>>
//{
//    public int? HotelId { get; set; }
//}

///// <summary>
///// Query to get assignment history for a specific order
///// </summary>
//public class GetOrderAssignmentHistoryQuery : IRequest<Result<IEnumerable<OrderAssignmentHistoryDto>>>
//{
//    public int OrderId { get; set; }
//}

///// <summary>
///// Query to get all attachments for a specific order
///// </summary>
//public class GetOrderAttachmentsQuery : IRequest<Result<IEnumerable<OrderAttachmentDto>>>
//{
//    public int OrderId { get; set; }
//}
