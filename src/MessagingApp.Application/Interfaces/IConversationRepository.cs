using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetDirectConversationAsync(Guid userAId, Guid userBId);
    Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid userId);
    Task AddAsync(Conversation conversation);
}
