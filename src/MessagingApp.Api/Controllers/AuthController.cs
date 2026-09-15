using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
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
}
