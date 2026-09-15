namespace MessagingApp.Application.DTOs;

/// <summary>A single message within a conversation.</summary>
/// <param name="Id">The message's unique identifier.</param>
/// <param name="ConversationId">The conversation this message belongs to.</param>
/// <param name="SenderId">The Id of the user who sent this message.</param>
/// <param name="Content">The message text.</param>
/// <param name="SentAt">UTC timestamp of when the message was sent.</param>
public record MessageResponse(Guid Id, Guid ConversationId, Guid SenderId, string Content, DateTime SentAt);
