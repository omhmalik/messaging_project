using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync(User.GetUserId());
        return Ok(result);
    }
}
