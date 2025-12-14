using FluentValidation;
using HotelMaintenance.Application.DTOs.Departments;
using HotelMaintenance.Application.DTOs.Hotels;
using HotelMaintenance.Application.DTOs.Locations;
using HotelMaintenance.Application.DTOs.SpareParts;
using HotelMaintenance.Application.DTOs.Users;

namespace HotelMaintenance.Application.Validators;

public class CreateHotelDtoValidator : AbstractValidator<CreateHotelDto>
{
    public CreateHotelDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Hotel code is required")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hotel name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address");

        RuleFor(x => x.StarRating)
            .InclusiveBetween(1, 5).When(x => x.StarRating.HasValue)
            .WithMessage("Star rating must be between 1 and 5");
    }
}

public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Hotel is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Department code is required")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid department type");
    }
}

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required")
            .MaximumLength(50).WithMessage("Employee ID cannot exceed 50 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Hotel is required");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department is required");
    }
}

/// <summary>
/// Validator for creating a new Item
/// </summary>
public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemDtoValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Item code is required")
            .MaximumLength(50).WithMessage("Item code must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Item code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name must not exceed 200 characters");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid category must be selected");

        RuleFor(x => x.LocationId)
            .GreaterThan(0).WithMessage("Valid location must be selected");

        RuleFor(x => x.MaintenanceIntervalDays)
            .GreaterThan(0).When(x => x.RequiresPreventiveMaintenance && x.MaintenanceIntervalDays.HasValue)
            .WithMessage("Maintenance interval must be greater than 0 when preventive maintenance is required");
    }
}

/// <summary>
/// Validator for creating a new Location
/// </summary>
public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Location code is required")
            .MaximumLength(50).WithMessage("Location code must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Location code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Location name is required")
            .MaximumLength(200).WithMessage("Location name must not exceed 200 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Valid location type must be selected");

        RuleFor(x => x.Building)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Building))
            .WithMessage("Building must not exceed 100 characters");

        RuleFor(x => x.Floor)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Floor))
            .WithMessage("Floor must not exceed 20 characters");

        RuleFor(x => x.Zone)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Zone))
            .WithMessage("Zone must not exceed 50 characters");

        RuleFor(x => x.RoomNumber)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.RoomNumber))
            .WithMessage("Room number must not exceed 20 characters");
    }
}
public class ItemDtoValidator : AbstractValidator<ItemDto>
{
    public ItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name must not exceed 200 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Item code is required")
            .MaximumLength(50).WithMessage("Item code must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Item code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid category must be selected");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");



        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Unit of measure is required")
            .MaximumLength(20).WithMessage("Unit of measure must not exceed 20 characters");


        RuleFor(x => x.Manufacturer)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Manufacturer))
            .WithMessage("Manufacturer name must not exceed 200 characters");

        RuleFor(x => x.Model)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Model))
            .WithMessage("Model number must not exceed 100 characters");


        RuleFor(x => x.LocationName)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.LocationName))
            .WithMessage("Storage location must not exceed 200 characters");
    }
}


/// <summary>
/// Validator for updating an existing Item
/// </summary>
public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name must not exceed 200 characters");

        RuleFor(x => x.LocationId)
            .GreaterThan(0).WithMessage("Valid location must be selected");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Valid item status must be selected");

        RuleFor(x => x.Manufacturer)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Manufacturer))
            .WithMessage("Manufacturer name must not exceed 200 characters");

        RuleFor(x => x.Model)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Model))
            .WithMessage("Model must not exceed 100 characters");

        RuleFor(x => x.SerialNumber)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.SerialNumber))
            .WithMessage("Serial number must not exceed 100 characters");
    }
}


/// <summary>
/// Validator for Spare Part DTO
/// </summary>
public class SparePartDtoValidator : AbstractValidator<SparePartDto>
{
    public SparePartDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Spare part name is required")
            .MaximumLength(200).WithMessage("Spare part name must not exceed 200 characters");

        RuleFor(x => x.PartNumber)
            .NotEmpty().WithMessage("Part number is required")
            .MaximumLength(100).WithMessage("Part number must not exceed 100 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Part number must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");

        RuleFor(x => x.QuantityOnHand)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level cannot be negative");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");


        RuleFor(x => x.Manufacturer)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Manufacturer))
            .WithMessage("Manufacturer name must not exceed 200 characters");

        RuleFor(x => x.StorageLocation)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.StorageLocation))
            .WithMessage("Storage location must not exceed 200 characters");
    }
}

/// <summary>
/// Validator for creating a new Spare Part
/// </summary>
public class CreateSparePartDtoValidator : AbstractValidator<CreateSparePartDto>
{
    public CreateSparePartDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Spare part code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Spare part name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");

        RuleFor(x => x.StorageDepartmentId)
            .GreaterThan(0).WithMessage("Valid storage department must be selected");

        RuleFor(x => x.PartNumber)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.PartNumber))
            .WithMessage("Part number must not exceed 100 characters");

        RuleFor(x => x.Manufacturer)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Manufacturer))
            .WithMessage("Manufacturer must not exceed 200 characters");

        RuleFor(x => x.QuantityOnHand)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity on hand cannot be negative");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity cannot be negative");

        RuleFor(x => x.ReorderQuantity)
            .GreaterThan(0).WithMessage("Reorder quantity must be greater than 0");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative");

        RuleFor(x => x.UnitOfMeasure)
            .NotEmpty().WithMessage("Unit of measure is required")
            .MaximumLength(20).WithMessage("Unit of measure must not exceed 20 characters");
    }
}

/// <summary>
/// Validator for updating an existing Spare Part
/// </summary>
public class UpdateSparePartDtoValidator : AbstractValidator<UpdateSparePartDto>
{
    public UpdateSparePartDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Spare part name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.PartNumber)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.PartNumber))
            .WithMessage("Part number must not exceed 100 characters");

        RuleFor(x => x.StorageLocation)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.StorageLocation))
            .WithMessage("Storage location must not exceed 200 characters");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity cannot be negative");

        RuleFor(x => x.ReorderQuantity)
            .GreaterThan(0).WithMessage("Reorder quantity must be greater than 0")
            .GreaterThan(x => x.MinimumQuantity)
            .WithMessage("Reorder quantity should be greater than minimum quantity");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative");
    }
}

/// <summary>
/// Validator for Location DTO
/// </summary>
public class LocationDtoValidator : AbstractValidator<LocationDto>
{
    public LocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Location name is required")
            .MaximumLength(200).WithMessage("Location name must not exceed 200 characters");


        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Valid hotel must be selected");

        RuleFor(x => x.Floor)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Floor))
            .WithMessage("Floor must not exceed 20 characters");

        RuleFor(x => x.Building)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Building))
            .WithMessage("Building must not exceed 100 characters");

        RuleFor(x => x.RoomNumber)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.RoomNumber))
            .WithMessage("Room number must not exceed 20 characters");

    }
}


/// <summary>
/// Validator for updating an existing Location
/// </summary>
public class UpdateLocationDtoValidator : AbstractValidator<UpdateLocationDto>
{
    public UpdateLocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Location name is required")
            .MaximumLength(200).WithMessage("Location name must not exceed 200 characters");

        RuleFor(x => x.Building)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Building))
            .WithMessage("Building must not exceed 100 characters");

        RuleFor(x => x.Floor)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Floor))
            .WithMessage("Floor must not exceed 20 characters");

        RuleFor(x => x.Zone)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Zone))
            .WithMessage("Zone must not exceed 50 characters");

        RuleFor(x => x.RoomNumber)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.RoomNumber))
            .WithMessage("Room number must not exceed 20 characters");
    }
}
