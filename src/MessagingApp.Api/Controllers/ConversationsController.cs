using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly IMessagingService _messagingService;

    public ConversationsController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConversationSummaryResponse>>> GetConversations([FromQuery] string? search)
    {
        var result = await _messagingService.GetConversationsAsync(User.GetUserId(), search);
        return Ok(result);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<IReadOnlyList<MessageResponse>>> GetMessages(
        Guid conversationId, [FromQuery] DateTime? before, [FromQuery] int take = 20)
    {
        var result = await _messagingService.GetMessagesAsync(User.GetUserId(), conversationId, before, take);
        return Ok(result);
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MessageResponse>> SendMessage(SendMessageRequest request)
    {
        var result = await _messagingService.SendMessageAsync(User.GetUserId(), request);
        return Ok(result);
    }
}
