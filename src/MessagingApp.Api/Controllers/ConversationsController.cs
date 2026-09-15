using MessagingApp.Api.Extensions;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.Controllers;

/// <summary>
/// Conversations and messages between the current user and others.
/// </summary>
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

    /// <summary>
    /// Lists the current user's conversations, newest activity first.
    /// </summary>
    /// <param name="search">Optional: filter to conversations whose other participant's display name contains this text.</param>
    /// <response code="200">The list of conversations, each with the other participant and a preview of the last message.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ConversationSummaryResponse>>> GetConversations([FromQuery] string? search)
    {
        var result = await _messagingService.GetConversationsAsync(User.GetUserId(), search);
        return Ok(result);
    }

    /// <summary>
    /// Gets a page of messages from one conversation, newest first, for cursor-based (infinite scroll) pagination.
    /// </summary>
    /// <param name="conversationId">The conversation to read messages from.</param>
    /// <param name="before">Optional: only return messages sent strictly before this UTC timestamp (pass the oldest timestamp from the previous page to keep scrolling back).</param>
    /// <param name="take">Maximum number of messages to return (default 20).</param>
    /// <response code="200">The requested page of messages, ordered oldest to newest.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    /// <response code="403">You are not a participant of this conversation.</response>
    [HttpGet("{conversationId}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<MessageResponse>>> GetMessages(
        Guid conversationId, [FromQuery] DateTime? before, [FromQuery] int take = 20)
    {
        var result = await _messagingService.GetMessagesAsync(User.GetUserId(), conversationId, before, take);
        return Ok(result);
    }

    /// <summary>
    /// Sends a message to another user. If no direct conversation exists between you and the
    /// recipient yet, one is created automatically — there is no separate "start conversation" step.
    /// </summary>
    /// <param name="request">The recipient's user Id and the message content.</param>
    /// <response code="200">The message was sent; returns the created message.</response>
    /// <response code="400">You attempted to message yourself, or the content failed validation.</response>
    /// <response code="401">No valid JWT was supplied.</response>
    /// <response code="404">The recipient does not exist.</response>
    [HttpPost("messages")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageResponse>> SendMessage(SendMessageRequest request)
    {
        var result = await _messagingService.SendMessageAsync(User.GetUserId(), request);
        return Ok(result);
    }
}
