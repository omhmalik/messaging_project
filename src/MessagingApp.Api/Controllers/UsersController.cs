using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

/// <summary>
/// User discovery endpoints.
/// </summary>
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

    /// <summary>
    /// Searches for users by a partial, case-insensitive match on their username.
    /// Used to find someone you haven't messaged before, to start a new conversation.
    /// </summary>
    /// <param name="query">Part of the username to search for.</param>
    /// <response code="200">Matching users (excluding yourself).</response>
    /// <response code="401">No valid JWT was supplied.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> Search([FromQuery] string query)
    {
        var result = await _messagingService.SearchUsersAsync(User.GetUserId(), query);
        return Ok(result);
    }
}
