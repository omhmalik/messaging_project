using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Interfaces;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, DateTime? before, int take);
    Task AddAsync(Message message);
}
