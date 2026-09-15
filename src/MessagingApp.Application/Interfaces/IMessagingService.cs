using MessagingApp.Application.DTOs;

namespace MessagingApp.Application.Interfaces;

public interface IMessagingService
{
    Task<IReadOnlyList<UserResponse>> SearchUsersAsync(Guid currentUserId, string query);
    Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request);
    Task<IReadOnlyList<ConversationSummaryResponse>> GetConversationsAsync(Guid userId, string? search);
    Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid userId, Guid conversationId, DateTime? before, int take);
}
