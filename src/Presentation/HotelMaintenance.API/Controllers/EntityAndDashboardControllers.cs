using HotelMaintenance.Application.DTOs.Items;
using HotelMaintenance.Application.DTOs.SpareParts;
using HotelMaintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMaintenance.API.Controllers;

/// <summary>
/// Items/Equipment management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IUnitOfWork unitOfWork, ILogger<ItemsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Items.View")]
    [ProducesResponseType(typeof(IEnumerable<ItemListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? hotelId = null,
        [FromQuery] int? locationId = null,
        [FromQuery] int? categoryId = null)
    {
        try
        {
            IEnumerable<Domain.Entities.Item> items;

            if (hotelId.HasValue)
                items = await _unitOfWork.Items.GetByHotelIdAsync(hotelId.Value);
            else if (locationId.HasValue)
                items = await _unitOfWork.Items.GetByLocationIdAsync(locationId.Value);
            else if (categoryId.HasValue)
                items = await _unitOfWork.Items.GetByCategoryIdAsync(categoryId.Value);
            else
                items = await _unitOfWork.Items.GetAllAsync();

            var result = items.Select(i => new ItemListDto
            {
                Id = i.Id,
                Code = i.Code,
                Name = i.Name,
                CategoryName = i.Category.Name,
                ClassName = i.Class?.Name,
                LocationName = i.Location?.Name,
                Status = i.Status,
                Manufacturer = i.Manufacturer,
                Model = i.Model,
                SerialNumber = i.SerialNumber,
                IsActive = i.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Items.View")]
    [ProducesResponseType(typeof(ItemDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            var result = new ItemDetailDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                CategoryName = item.Category.Name,
                ClassId = item.ClassId,
                ClassName = item.Class?.Name,
                FamilyId = item.FamilyId,
                FamilyName = item.Family?.Name,
                LocationId = item.LocationId,
                LocationName = item.Location?.Name,
                HotelId = item.HotelId,
                HotelName = item.Hotel.Name,
                Status = item.Status,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                SerialNumber = item.SerialNumber,
                AssetTag = item.AssetTag,
                InstallationDate = item.InstallationDate,
                WarrantyExpiryDate = item.WarrantyExpiryDate,
                PurchaseCost = item.PurchaseCost,
                CurrentValue = item.CurrentValue,
                IsActive = item.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item {ItemId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _unitOfWork.ItemCategories.GetAllAsync();
            var result = categories.Where(c => c.IsActive).Select(c => new
            {
                c.Id,
                c.Code,
                c.Name,
                c.Description,
                c.Icon
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item categories");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Spare Parts management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SparePartsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SparePartsController> _logger;

    public SparePartsController(IUnitOfWork unitOfWork, ILogger<SparePartsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "SpareParts.View")]
    [ProducesResponseType(typeof(IEnumerable<SparePartListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? hotelId = null)
    {
        try
        {
            var parts = hotelId.HasValue
                ? await _unitOfWork.SpareParts.GetByHotelIdAsync(hotelId.Value)
                : await _unitOfWork.SpareParts.GetAllAsync();

            var result = parts.Select(s => new SparePartListDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                PartNumber = s.PartNumber,
                Manufacturer = s.Manufacturer,
                QuantityOnHand = s.QuantityOnHand,
                MinimumQuantity = s.MinimumQuantity,
                MaximumQuantity = s.MaximumQuantity,
                UnitCost = s.UnitCost,
                IsCritical = s.IsCritical,
                ItemName = s.Item?.Name,
                IsActive = s.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting spare parts");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SpareParts.View")]
    [ProducesResponseType(typeof(SparePartDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var part = await _unitOfWork.SpareParts.GetByIdAsync(id);
            if (part == null)
                return NotFound();

            var result = new SparePartDetailDto
            {
                Id = part.Id,
                Code = part.Code,
                Name = part.Name,
                Description = part.Description,
                PartNumber = part.PartNumber,
                Manufacturer = part.Manufacturer,
                ItemId = part.ItemId,
                ItemName = part.Item?.Name,
                HotelId = part.HotelId,
                StorageLocation = part.StorageLocation,
                BinLocation = part.BinLocation,
                QuantityOnHand = part.QuantityOnHand,
                MinimumQuantity = part.MinimumQuantity,
                MaximumQuantity = part.MaximumQuantity,
                ReorderPoint = part.ReorderPoint,
                UnitOfMeasure = part.UnitOfMeasure,
                UnitCost = part.UnitCost,
                LastPurchasePrice = part.LastPurchasePrice,
                IsCritical = part.IsCritical,
                IsActive = part.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting spare part {PartId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("low-stock")]
    [Authorize(Policy = "SpareParts.View")]
    [ProducesResponseType(typeof(IEnumerable<SparePartListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockParts([FromQuery] int hotelId)
    {
        try
        {
            var parts = await _unitOfWork.SpareParts.GetLowStockPartsAsync(hotelId);
            var result = parts.Select(s => new SparePartListDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                PartNumber = s.PartNumber,
                QuantityOnHand = s.QuantityOnHand,
                MinimumQuantity = s.MinimumQuantity,
                IsCritical = s.IsCritical,
                ItemName = s.Item?.Name
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock parts");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Vendors management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VendorsController> _logger;

    public VendorsController(IUnitOfWork unitOfWork, ILogger<VendorsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var vendors = await _unitOfWork.Vendors.GetActiveVendorsAsync();
            var result = vendors.Select(v => new
            {
                v.Id,
                v.Code,
                v.Name,
                v.Type,
                v.ContactPerson,
                v.Email,
                v.Phone,
                v.IsPreferred,
                v.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendors");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (vendor == null)
                return NotFound();

            var result = new
            {
                vendor.Id,
                vendor.Code,
                vendor.Name,
                vendor.Type,
                vendor.Description,
                vendor.ContactPerson,
                vendor.Email,
                vendor.Phone,
                vendor.Mobile,
                vendor.Website,
                vendor.Address,
                vendor.City,
                vendor.State,
                vendor.Country,
                vendor.ServiceRating,
                vendor.IsPreferred,
                vendor.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor {VendorId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Dashboard and statistics endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Policy = "Dashboard.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics([FromQuery] int? hotelId = null)
    {
        try
        {
            var allOrders = hotelId.HasValue
                ? await _unitOfWork.MaintenanceOrders.GetOrdersByHotelAsync(hotelId.Value)
                : await _unitOfWork.MaintenanceOrders.GetAllAsync();

            var overdueOrders = await _unitOfWork.MaintenanceOrders.GetOverdueOrdersAsync();
            var slaBreachedOrders = await _unitOfWork.MaintenanceOrders.GetSLABreachedOrdersAsync();

            var statistics = new
            {
                TotalOrders = allOrders.Count(),
                OpenOrders = allOrders.Count(o => o.CurrentStatus != Domain.Enums.OrderStatus.Completed && 
                                                    o.CurrentStatus != Domain.Enums.OrderStatus.Closed &&
                                                    o.CurrentStatus != Domain.Enums.OrderStatus.Cancelled),
                CompletedOrders = allOrders.Count(o => o.CurrentStatus == Domain.Enums.OrderStatus.Completed),
                OverdueOrders = overdueOrders.Count(),
                SLABreachedOrders = slaBreachedOrders.Count(),
                OrdersByPriority = new
                {
                    Critical = allOrders.Count(o => o.Priority == Domain.Enums.OrderPriority.Critical),
                    High = allOrders.Count(o => o.Priority == Domain.Enums.OrderPriority.High),
                    Medium = allOrders.Count(o => o.Priority == Domain.Enums.OrderPriority.Medium),
                    Low = allOrders.Count(o => o.Priority == Domain.Enums.OrderPriority.Low)
                },
                OrdersByStatus = allOrders.GroupBy(o => o.CurrentStatus)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() }),
                AverageCost = allOrders.Any() ? allOrders.Average(o => o.ActualCost) : 0,
                TotalCost = allOrders.Sum(o => o.ActualCost)
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard statistics");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get recent activity
    /// </summary>
    [HttpGet("recent-activity")]
    [Authorize(Policy = "Dashboard.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int? hotelId = null, [FromQuery] int limit = 10)
    {
        try
        {
            var orders = hotelId.HasValue
                ? await _unitOfWork.MaintenanceOrders.GetOrdersByHotelAsync(hotelId.Value)
                : await _unitOfWork.MaintenanceOrders.GetAllAsync();

            var recentOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNumber,
                    o.Title,
                    o.Priority,
                    o.CurrentStatus,
                    HotelName = o.Hotel.Name,
                    AssignedToUserName = o.AssignedToUser?.FullName,
                    o.CreatedAt
                });

            return Ok(recentOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activity");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get order trends
    /// </summary>
    [HttpGet("trends")]
    [Authorize(Policy = "Dashboard.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrends([FromQuery] int? hotelId = null, [FromQuery] int days = 30)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var orders = hotelId.HasValue
                ? await _unitOfWork.MaintenanceOrders.GetOrdersByHotelAsync(hotelId.Value)
                : await _unitOfWork.MaintenanceOrders.GetAllAsync();

            var ordersInRange = orders.Where(o => o.CreatedAt >= startDate);

            var trends = ordersInRange
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Completed = g.Count(o => o.CurrentStatus == Domain.Enums.OrderStatus.Completed)
                })
                .OrderBy(t => t.Date);

            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trends");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}
