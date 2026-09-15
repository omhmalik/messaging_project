namespace MessagingApp.Application.DTOs;

public record MessageResponse(Guid Id, Guid ConversationId, Guid SenderId, string Content, DateTime SentAt);
