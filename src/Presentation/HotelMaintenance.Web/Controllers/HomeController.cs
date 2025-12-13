using HotelMaintenance.Web.Models;
using HotelMaintenance.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelMaintenance.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApiClient apiClient, ILogger<HomeController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // Check if user is logged in
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Get user info
            var userInfoJson = HttpContext.Session.GetString("UserInfo");
            var userInfo = JsonConvert.DeserializeObject<UserInfoViewModel>(userInfoJson ?? "{}");

            // Get dashboard statistics
            var statistics = await _apiClient.GetAsync<DashboardViewModel>(
                $"/api/Dashboard/statistics?hotelId={userInfo?.HotelId}");

            // Get recent activity
            var recentOrders = await _apiClient.GetAsync<List<RecentOrderViewModel>>(
                $"/api/Dashboard/recent-activity?hotelId={userInfo?.HotelId}&limit=10");

            // Get trends
            var trends = await _apiClient.GetAsync<List<OrderTrendViewModel>>(
                $"/api/Dashboard/trends?hotelId={userInfo?.HotelId}&days=30");

            var model = statistics ?? new DashboardViewModel();
            model.RecentOrders = recentOrders ?? new List<RecentOrderViewModel>();
            model.OrderTrends = trends ?? new List<OrderTrendViewModel>();

            ViewBag.UserInfo = userInfo;

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View(new DashboardViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}