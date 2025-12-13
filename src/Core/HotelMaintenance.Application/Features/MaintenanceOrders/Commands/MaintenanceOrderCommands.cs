using HotelMaintenance.Application.Common;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using MediatR;

namespace HotelMaintenance.Application.Features.MaintenanceOrders.Commands;

// Create Order Command
public record CreateMaintenanceOrderCommand(CreateMaintenanceOrderDto OrderDto, int CreatedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Update Order Command
public record UpdateMaintenanceOrderCommand(long OrderId, UpdateMaintenanceOrderDto OrderDto, int ModifiedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Assign Order Command
public record AssignMaintenanceOrderCommand(long OrderId, AssignOrderDto AssignDto, int AssignedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Update Status Command
public record UpdateOrderStatusCommand(long OrderId, UpdateOrderStatusDto StatusDto, int ChangedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Complete Order Command
public record CompleteMaintenanceOrderCommand(long OrderId, CompleteOrderDto CompleteDto, int CompletedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Verify Order Command (by requester)
public record VerifyMaintenanceOrderCommand(long OrderId, VerifyOrderDto VerifyDto, int VerifiedByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Cancel Order Command
public record CancelMaintenanceOrderCommand(long OrderId, CancelOrderDto CancelDto, int CancelledByUserId) 
    : IRequest<Result<MaintenanceOrderDto>>;

// Add Comment Command
public record AddOrderCommentCommand(CreateOrderCommentDto CommentDto, int UserId) 
    : IRequest<Result<OrderCommentDto>>;

// Upload Attachment Command
public record UploadOrderAttachmentCommand(
    long OrderId, 
    string FileName, 
    string FileUrl, 
    long FileSize,
    string FileType,
    string Description,
    int UploadedByUserId) 
    : IRequest<Result<OrderAttachmentDto>>;
