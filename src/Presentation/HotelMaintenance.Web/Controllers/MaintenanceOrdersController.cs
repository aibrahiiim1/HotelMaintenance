using HotelMaintenance.Web.Models;
using HotelMaintenance.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelMaintenance.Web.Controllers;

public class MaintenanceOrdersController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<MaintenanceOrdersController> _logger;

    public MaintenanceOrdersController(ApiClient apiClient, ILogger<MaintenanceOrdersController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// List all maintenance orders
    /// </summary>
    public async Task<IActionResult> Index(string? status = null, int? priority = null)
    {
        // Check if user is logged in
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var endpoint = "/api/MaintenanceOrders";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(status))
                queryParams.Add($"status={status}");
            if (priority.HasValue)
                queryParams.Add($"priority={priority}");

            if (queryParams.Any())
                endpoint += "?" + string.Join("&", queryParams);

            var orders = await _apiClient.GetAsync<List<MaintenanceOrderListViewModel>>(endpoint);

            ViewBag.Status = status;
            ViewBag.Priority = priority;

            return View(orders ?? new List<MaintenanceOrderListViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading maintenance orders");
            return View(new List<MaintenanceOrderListViewModel>());
        }
    }

    /// <summary>
    /// View order details
    /// </summary>
    public async Task<IActionResult> Details(long id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var order = await _apiClient.GetAsync<MaintenanceOrderDetailViewModel>($"/api/MaintenanceOrders/{id}");

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order {OrderId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// Create new order - GET
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            await LoadCreateFormData();
            return View(new CreateOrderViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create form");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Create new order - POST
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            await LoadCreateFormData();
            return View(model);
        }

        try
        {
            var result = await _apiClient.PostAsync<CreateOrderViewModel, object>(
                "/api/MaintenanceOrders", model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Maintenance order created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to create order");
            await LoadCreateFormData();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the order");
            await LoadCreateFormData();
            return View(model);
        }
    }

    /// <summary>
    /// My assigned orders
    /// </summary>
    public async Task<IActionResult> MyOrders()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var orders = await _apiClient.GetAsync<List<MaintenanceOrderListViewModel>>(
                "/api/MaintenanceOrders/my-orders");

            return View(orders ?? new List<MaintenanceOrderListViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading my orders");
            return View(new List<MaintenanceOrderListViewModel>());
        }
    }

    /// <summary>
    /// Overdue orders
    /// </summary>
    public async Task<IActionResult> Overdue()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var orders = await _apiClient.GetAsync<List<MaintenanceOrderListViewModel>>(
                "/api/MaintenanceOrders/overdue");

            return View(orders ?? new List<MaintenanceOrderListViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading overdue orders");
            return View(new List<MaintenanceOrderListViewModel>());
        }
    }

    /// <summary>
    /// SLA breached orders
    /// </summary>
    public async Task<IActionResult> SLABreached()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var orders = await _apiClient.GetAsync<List<MaintenanceOrderListViewModel>>(
                "/api/MaintenanceOrders/sla-breached");

            return View(orders ?? new List<MaintenanceOrderListViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading SLA breached orders");
            return View(new List<MaintenanceOrderListViewModel>());
        }
    }

    /// <summary>
    /// Assign order - POST
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Assign(long orderId, int assignedToUserId, int assignedDepartmentId, string? notes)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return Json(new { success = false, message = "Not authenticated" });
        }

        try
        {
            var assignData = new { assignedToUserId, assignedDepartmentId, notes };
            var result = await _apiClient.PostAsync<object, object>(
                $"/api/MaintenanceOrders/{orderId}/assign", assignData);

            if (result != null)
            {
                return Json(new { success = true, message = "Order assigned successfully" });
            }

            return Json(new { success = false, message = "Failed to assign order" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning order {OrderId}", orderId);
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Change order status - POST
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ChangeStatus(long orderId, int newStatus, string? notes)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return Json(new { success = false, message = "Not authenticated" });
        }

        try
        {
            var statusData = new { newStatus, notes };
            var result = await _apiClient.PostAsync<object, object>(
                $"/api/MaintenanceOrders/{orderId}/status", statusData);

            if (result != null)
            {
                return Json(new { success = true, message = "Status changed successfully" });
            }

            return Json(new { success = false, message = "Failed to change status" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing status for order {OrderId}", orderId);
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Add comment - POST
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddComment(long orderId, string commentText, bool isInternal)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return Json(new { success = false, message = "Not authenticated" });
        }

        try
        {
            var commentData = new { commentText, isInternal };
            var result = await _apiClient.PostAsync<object, object>(
                $"/api/MaintenanceOrders/{orderId}/comments", commentData);

            if (result != null)
            {
                return Json(new { success = true, message = "Comment added successfully" });
            }

            return Json(new { success = false, message = "Failed to add comment" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to order {OrderId}", orderId);
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Helper method to load dropdown data for create form
    /// </summary>
    private async Task LoadCreateFormData()
    {
        var userInfoJson = HttpContext.Session.GetString("UserInfo");
        var userInfo = JsonConvert.DeserializeObject<UserInfoViewModel>(userInfoJson ?? "{}");

        // Get hotels
        var hotels = await _apiClient.GetAsync<List<object>>("/api/Hotels/active");
        ViewBag.Hotels = hotels ?? new List<object>();

        // Get departments
        var departments = await _apiClient.GetAsync<List<object>>(
            $"/api/Departments?hotelId={userInfo?.HotelId}");
        ViewBag.Departments = departments ?? new List<object>();

        // Get locations
        var locations = await _apiClient.GetAsync<List<object>>(
            $"/api/Locations?hotelId={userInfo?.HotelId}");
        ViewBag.Locations = locations ?? new List<object>();

        // Get items
        var items = await _apiClient.GetAsync<List<object>>(
            $"/api/Items?hotelId={userInfo?.HotelId}");
        ViewBag.Items = items ?? new List<object>();

        ViewBag.UserInfo = userInfo;
    }
}