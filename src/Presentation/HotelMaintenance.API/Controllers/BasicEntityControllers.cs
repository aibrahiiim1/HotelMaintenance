using HotelMaintenance.Application.DTOs.Hotels;
using HotelMaintenance.Application.DTOs.Departments;
using HotelMaintenance.Application.DTOs.Users;
using HotelMaintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMaintenance.API.Controllers;

/// <summary>
/// Hotels management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(IUnitOfWork unitOfWork, ILogger<HotelsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HotelListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var hotels = await _unitOfWork.Hotels.GetAllAsync();
            var result = hotels.Select(h => new HotelListDto
            {
                Id = h.Id,
                Code = h.Code,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                IsActive = h.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotels");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HotelDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                return NotFound();

            var result = new HotelDetailDto
            {
                Id = hotel.Id,
                Code = hotel.Code,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                State = hotel.State,
                Country = hotel.Country,
                PostalCode = hotel.PostalCode,
                Phone = hotel.Phone,
                Email = hotel.Email,
                TimeZone = hotel.TimeZone,
                IsActive = hotel.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotel {HotelId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<HotelListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveHotels()
    {
        try
        {
            var hotels = await _unitOfWork.Hotels.GetActiveHotelsAsync();
            var result = hotels.Select(h => new HotelListDto
            {
                Id = h.Id,
                Code = h.Code,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                IsActive = h.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active hotels");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Departments management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(IUnitOfWork unitOfWork, ILogger<DepartmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? hotelId = null)
    {
        try
        {
            var departments = hotelId.HasValue
                ? await _unitOfWork.Departments.GetByHotelIdAsync(hotelId.Value)
                : await _unitOfWork.Departments.GetAllAsync();

            var result = departments.Select(d => new DepartmentListDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                ManagerName = d.Manager.FullName,
                HotelName = d.Hotel.Name,
                TypeName = d.Type.ToString(),
                IsActive = d.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DepartmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            var result = new DepartmentDetailDto
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                Description = department.Description,
                Type = department.Type,
                HotelId = department.HotelId,
                HotelName = department.Hotel.Name,
                ManagerUserId = department.ManagerUserId,
                ManagerName = department.Manager?.FullName,
                IsMaintenanceProvider = department.IsMaintenanceProvider,
                IsActive = department.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {DeptId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Users management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Users.View")]
    [ProducesResponseType(typeof(IEnumerable<UserListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? hotelId = null, [FromQuery] int? departmentId = null)
    {
        try
        {
            IEnumerable<Domain.Entities.User> users;
            
            if (departmentId.HasValue)
            {
                users = await _unitOfWork.Users.GetAvailableTechniciansAsync(departmentId.Value);
            }
            else
            {
                users = await _unitOfWork.Users.GetAllAsync();
                if (hotelId.HasValue)
                {
                    users = users.Where(u => u.HotelId == hotelId.Value);
                }
            }

            var result = users.Select(u => new UserListDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                JobTitle = u.JobTitle,
                DepartmentName = u.Department.Name,
                HotelName = u.Hotel.Name,
                IsActive = u.IsActive,
                IsAvailable = u.IsAvailable
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Users.View")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = new UserDetailDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Mobile = user.Mobile,
                JobTitle = user.JobTitle,
                HotelId = user.HotelId,
                HotelName = user.Hotel.Name,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department.Name,
                IsActive = user.IsActive,
                IsAvailable = user.IsAvailable,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("available-technicians/{departmentId}")]
    [Authorize(Policy = "Orders.Assign")]
    [ProducesResponseType(typeof(IEnumerable<UserListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableTechnicians(int departmentId)
    {
        try
        {
            var technicians = await _unitOfWork.Users.GetAvailableTechniciansAsync(departmentId);
            var result = technicians.Select(u => new UserListDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FullName = u.FullName,
                Email = u.Email,
                JobTitle = u.JobTitle,
                IsAvailable = u.IsAvailable
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available technicians");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}

/// <summary>
/// Locations management endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(IUnitOfWork unitOfWork, ILogger<LocationsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? hotelId = null)
    {
        try
        {
            var locations = hotelId.HasValue
                ? await _unitOfWork.Locations.GetByHotelIdAsync(hotelId.Value)
                : await _unitOfWork.Locations.GetAllAsync();

            var result = locations.Select(l => new
            {
                l.Id,
                l.Code,
                l.Name,
                l.Type,
                l.Building,
                l.Floor,
                l.RoomNumber,
                HotelName = l.Hotel.Name,
                ParentLocationName = l.ParentLocation?.Name,
                l.IsActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locations");
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
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            if (location == null)
                return NotFound();

            var result = new
            {
                location.Id,
                location.Code,
                location.Name,
                location.Description,
                location.Type,
                location.Building,
                location.Floor,
                location.Zone,
                location.RoomNumber,
                location.HotelId,
                HotelName = location.Hotel.Name,
                location.ParentLocationId,
                ParentLocationName = location.ParentLocation?.Name,
                location.IsActive
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location {LocationId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}
