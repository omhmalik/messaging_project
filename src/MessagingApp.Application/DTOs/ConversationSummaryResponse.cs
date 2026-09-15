namespace MessagingApp.Application.DTOs;

/// <summary>A summary of one conversation, as shown in the conversation list.</summary>
/// <param name="ConversationId">The conversation's unique identifier.</param>
/// <param name="OtherUser">The other participant in this 1:1 conversation.</param>
/// <param name="LastMessage">The text of the most recent message, or null if no messages have been sent yet.</param>
/// <param name="LastMessageAt">UTC timestamp of the most recent message, or null if no messages have been sent yet.</param>
public record ConversationSummaryResponse(Guid ConversationId, UserResponse OtherUser, string? LastMessage, DateTime? LastMessageAt);
