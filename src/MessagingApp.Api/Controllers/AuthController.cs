using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Exceptions;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

/// <summary>
/// Registration, login, and current-user identity endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="request">Display name, username, email, and password for the new account.</param>
    /// <response code="200">Account created successfully.</response>
    /// <response code="400">One or more fields failed validation.</response>
    /// <response code="409">The email or username is already taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user and issues a JWT access token.
    /// </summary>
    /// <param name="request">The account's email and password.</param>
    /// <response code="200">Login succeeded; returns the JWT and the user's profile.</response>
    /// <response code="401">The email or password is incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// </summary>
    /// <response code="200">The current user's profile.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync(User.GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Updates the current user's display name, username, and email. The user's Id never
    /// changes, so existing conversations keep working and simply show the updated info.
    /// </summary>
    /// <param name="request">The new display name, username, and email.</param>
    /// <response code="200">Profile updated; returns the updated profile.</response>
    /// <response code="400">One or more fields failed validation.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    /// <response code="409">The email or username is already taken by another user.</response>
    [Authorize]
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await _authService.UpdateProfileAsync(User.GetUserId(), request);
        return Ok(result);
    }

    private static readonly string[] AllowedPictureExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxPictureSizeBytes = 5 * 1024 * 1024;

    /// <summary>
    /// Uploads or replaces the current user's profile picture.
    /// </summary>
    /// <param name="file">An image file (.jpg, .jpeg, .png, or .webp), up to 5 MB.</param>
    /// <response code="200">Picture uploaded; returns the updated profile.</response>
    /// <response code="400">The file is missing, too large, or not a supported image type.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    [Authorize]
    [HttpPost("me/picture")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> UpdateProfilePicture(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new BadRequestException("No file was uploaded.");
        }

        if (file.Length > MaxPictureSizeBytes)
        {
            throw new BadRequestException("The file must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedPictureExtensions.Contains(extension))
        {
            throw new BadRequestException("Only .jpg, .jpeg, .png, and .webp files are allowed.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _authService.UpdateProfilePictureAsync(User.GetUserId(), stream, extension);
        return Ok(result);
    }
}
