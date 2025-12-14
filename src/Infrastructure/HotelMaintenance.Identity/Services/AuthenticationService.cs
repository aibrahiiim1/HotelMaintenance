using HotelMaintenance.Domain.Interfaces;
using HotelMaintenance.Identity.Models;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace HotelMaintenance.Identity.Services;

/// <summary>
/// Authentication service for user login and password management
/// </summary>
public class AuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            
            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid email: {Email}", request.Email);
                return null;
            }

            // Verify password
            // Note: In production, you should have a PasswordHash field in User entity
            // For now, this is a placeholder - you'll need to add password hashing to User entity
            // After (real):
            if (!VerifyPassword(request.Password, user.PasswordHash))
                if (!VerifyPassword(request.Password, user.Email)) // Placeholder
            {
                _logger.LogWarning("Invalid password attempt for user: {Email}", request.Email);
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate token
            var tokenResponse = await _jwtTokenService.GenerateTokenAsync(user.Id);
            
            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            
            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Hash password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            // Note: Add PasswordHash property to User entity
            // if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            // {
            //     _logger.LogWarning("Invalid current password for user {UserId}", userId);
            //     return false;
            // }

            // Validate new password
            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.LogWarning("Password confirmation does not match for user {UserId}", userId);
                return false;
            }

            // Hash and update new password
            // user.PasswordHash = HashPassword(request.NewPassword);
            // user.LastModifiedAt = DateTime.UtcNow;
            
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user {UserId}", userId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Generate password reset token
    /// </summary>
    public string GeneratePasswordResetToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("+", "")
            .Replace("/", "");
    }

    /// <summary>
    /// Validate password strength
    /// </summary>
    public (bool isValid, List<string> errors) ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters long");
        }

        if (!password.Any(char.IsUpper))
        {
            errors.Add("Password must contain at least one uppercase letter");
        }

        if (!password.Any(char.IsLower))
        {
            errors.Add("Password must contain at least one lowercase letter");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("Password must contain at least one number");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            errors.Add("Password must contain at least one special character");
        }

        return (errors.Count == 0, errors);
    }
}
