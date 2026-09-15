using MessagingApp.Application.Interfaces;
using MessagingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MessagingAppDbContext _context;

    public MessageRepository(MessagingAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, DateTime? before, int take)
    {
        var query = _context.Messages.Where(m => m.ConversationId == conversationId);

        if (before.HasValue)
        {
            query = query.Where(m => m.SentAt < before.Value);
        }

        return await query
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }
}
