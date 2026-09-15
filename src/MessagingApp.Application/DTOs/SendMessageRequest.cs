namespace MessagingApp.Application.DTOs;

public record SendMessageRequest(Guid RecipientId, string Content);
