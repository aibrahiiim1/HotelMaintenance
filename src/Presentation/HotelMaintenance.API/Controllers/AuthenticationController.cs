using HotelMaintenance.Identity.Models;
using HotelMaintenance.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMaintenance.API.Controllers;

/// <summary>
/// Authentication endpoints for login, registration, and password management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        AuthenticationService authService,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _authService.LoginAsync(request);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Get current user information from token
    /// </summary>
    /// <returns>Current user details</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = _jwtTokenService.GetUserIdFromClaims(User);
            var response = await _jwtTokenService.GenerateTokenAsync(userId);
            
            return Ok(response.User);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success status</returns>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _jwtTokenService.GetUserIdFromClaims(User);
            var result = await _authService.ChangePasswordAsync(userId, request);
            
            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please check your current password." });
            }

            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Validate password strength
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ValidatePassword([FromBody] string password)
    {
        var (isValid, errors) = _authService.ValidatePassword(password);
        
        return Ok(new 
        { 
            isValid, 
            errors 
        });
    }

    /// <summary>
    /// Logout user (client-side token removal)
    /// </summary>
    /// <returns>Success message</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // JWT tokens are stateless, so logout is handled client-side
        // This endpoint is for logging purposes
        var userId = _jwtTokenService.GetUserIdFromClaims(User);
        _logger.LogInformation("User {UserId} logged out", userId);
        
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validate the old token
            var principal = _jwtTokenService.ValidateToken(request.Token);
            
            if (principal == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Get user ID from token
            var userId = _jwtTokenService.GetUserIdFromClaims(principal);
            
            // Generate new token
            var response = await _jwtTokenService.GenerateTokenAsync(userId);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Unauthorized(new { message = "Failed to refresh token" });
        }
    }
}
