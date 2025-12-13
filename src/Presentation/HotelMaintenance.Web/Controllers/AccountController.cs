using HotelMaintenance.Web.Models;
using HotelMaintenance.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelMaintenance.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApiClient apiClient, ILogger<AccountController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Redirect to home if already logged in
        if (HttpContext.Session.GetString("JwtToken") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, token, user, error) = await _apiClient.LoginAsync(model.Email, model.Password);

        if (success && token != null && user != null)
        {
            // Store JWT token in session
            HttpContext.Session.SetString("JwtToken", token);
            HttpContext.Session.SetString("UserInfo", JsonConvert.SerializeObject(user));

            _logger.LogInformation("User {Email} logged in successfully", model.Email);

            // Redirect to return URL or home
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, error ?? "Login failed");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        _logger.LogInformation("User logged out");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}