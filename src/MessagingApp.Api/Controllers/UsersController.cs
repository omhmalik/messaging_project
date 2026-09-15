using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMessagingService _messagingService;

    public UsersController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> Search([FromQuery] string query)
    {
        var result = await _messagingService.SearchUsersAsync(User.GetUserId(), query);
        return Ok(result);
    }
}
