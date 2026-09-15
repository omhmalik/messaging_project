using MessagingApp.Application.Interfaces;
using MessagingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Persistence.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly MessagingAppDbContext _context;

    public ConversationRepository(MessagingAppDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetDirectConversationAsync(Guid userAId, Guid userBId)
    {
        return await _context.Conversations
            .Where(c => c.Participants.Count == 2 &&
                        c.Participants.Any(p => p.UserId == userAId) &&
                        c.Participants.Any(p => p.UserId == userBId))
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId)
    {
        return await _context.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
            .ToListAsync();
    }

    public async Task<bool> IsParticipantAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task AddAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
    }
}
