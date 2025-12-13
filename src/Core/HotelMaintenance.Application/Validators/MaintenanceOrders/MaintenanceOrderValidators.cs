using FluentValidation;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using HotelMaintenance.Domain.Enums;

namespace HotelMaintenance.Application.Validators.MaintenanceOrders;

public class CreateMaintenanceOrderDtoValidator : AbstractValidator<CreateMaintenanceOrderDto>
{
    public CreateMaintenanceOrderDtoValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Hotel is required");

        RuleFor(x => x.RequestingDepartmentId)
            .GreaterThan(0).WithMessage("Requesting department is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid order type");

        RuleFor(x => x.LocationId)
            .GreaterThan(0).WithMessage("Location is required");

        RuleFor(x => x.ExpectedCompletionDate)
            .GreaterThan(DateTime.Now).WithMessage("Expected completion date must be in the future");
    }
}

public class UpdateMaintenanceOrderDtoValidator : AbstractValidator<UpdateMaintenanceOrderDto>
{
    public UpdateMaintenanceOrderDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority");

        RuleFor(x => x.LocationId)
            .GreaterThan(0).WithMessage("Location is required");
    }
}

public class AssignOrderDtoValidator : AbstractValidator<AssignOrderDto>
{
    public AssignOrderDtoValidator()
    {
        RuleFor(x => x.AssignedDepartmentId)
            .GreaterThan(0).WithMessage("Assigned department is required");
    }
}

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid status");

        RuleFor(x => x.ResolutionNotes)
            .NotEmpty()
            .When(x => x.NewStatus == OrderStatus.Completed || x.NewStatus == OrderStatus.Closed)
            .WithMessage("Resolution notes are required when completing an order");
    }
}

public class CompleteOrderDtoValidator : AbstractValidator<CompleteOrderDto>
{
    public CompleteOrderDtoValidator()
    {
        RuleFor(x => x.ResolutionNotes)
            .NotEmpty().WithMessage("Resolution notes are required")
            .MaximumLength(2000).WithMessage("Resolution notes cannot exceed 2000 characters");

        RuleFor(x => x.FollowUpDate)
            .GreaterThan(DateTime.Now)
            .When(x => x.RequiresFollowUp)
            .WithMessage("Follow-up date must be in the future when follow-up is required");

        RuleFor(x => x.LaborCost)
            .GreaterThanOrEqualTo(0).WithMessage("Labor cost must be non-negative");

        RuleFor(x => x.MaterialCost)
            .GreaterThanOrEqualTo(0).WithMessage("Material cost must be non-negative");
    }
}

public class VerifyOrderDtoValidator : AbstractValidator<VerifyOrderDto>
{
    public VerifyOrderDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Feedback)
            .MaximumLength(1000).WithMessage("Feedback cannot exceed 1000 characters");
    }
}

public class CancelOrderDtoValidator : AbstractValidator<CancelOrderDto>
{
    public CancelOrderDtoValidator()
    {
        RuleFor(x => x.CancellationReason)
            .NotEmpty().WithMessage("Cancellation reason is required")
            .MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters");
    }
}

public class CreateOrderCommentDtoValidator : AbstractValidator<CreateOrderCommentDto>
{
    public CreateOrderCommentDtoValidator()
    {
        RuleFor(x => x.MaintenanceOrderId)
            .GreaterThan(0).WithMessage("Order ID is required");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
    }
}
