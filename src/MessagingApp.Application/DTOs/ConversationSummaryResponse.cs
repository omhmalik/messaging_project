namespace MessagingApp.Application.DTOs;

public record ConversationSummaryResponse(Guid ConversationId, UserResponse OtherUser, string? LastMessage, DateTime? LastMessageAt);
