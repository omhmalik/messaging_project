using MessagingApp.Domain.Entities;
using MessagingApp.Infrastructure.Persistence;
using MessagingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace MessagingApp.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class ConversationRepositoryTests : IDisposable
{
    private readonly MessagingAppDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly ConversationRepository _sut;

    public ConversationRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _transaction = _context.Database.BeginTransaction();
        _sut = new ConversationRepository(_context);
    }

    [Fact]
    public async Task GetDirectConversationAsync_WhenExactlyTwoUsersShareAConversation_ReturnsIt()
    {
        var userA = CreateUser();
        var userB = CreateUser();
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Participants =
            [
                new ConversationParticipant { UserId = userA.Id, JoinedAt = DateTime.UtcNow },
                new ConversationParticipant { UserId = userB.Id, JoinedAt = DateTime.UtcNow }
            ]
        };
        _context.Users.AddRange(userA, userB);
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDirectConversationAsync(userA.Id, userB.Id);

        Assert.NotNull(result);
        Assert.Equal(conversation.Id, result!.Id);
    }

    [Fact]
    public async Task GetDirectConversationAsync_WhenNoSharedConversationExists_ReturnsNull()
    {
        var userA = CreateUser();
        var userB = CreateUser();
        _context.Users.AddRange(userA, userB);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDirectConversationAsync(userA.Id, userB.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task IsParticipantAsync_ForActualParticipant_ReturnsTrue()
    {
        var userA = CreateUser();
        var userB = CreateUser();
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Participants =
            [
                new ConversationParticipant { UserId = userA.Id, JoinedAt = DateTime.UtcNow },
                new ConversationParticipant { UserId = userB.Id, JoinedAt = DateTime.UtcNow }
            ]
        };
        _context.Users.AddRange(userA, userB);
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        var result = await _sut.IsParticipantAsync(conversation.Id, userA.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task IsParticipantAsync_ForAnOutsider_ReturnsFalse()
    {
        var userA = CreateUser();
        var userB = CreateUser();
        var outsider = CreateUser();
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Participants =
            [
                new ConversationParticipant { UserId = userA.Id, JoinedAt = DateTime.UtcNow },
                new ConversationParticipant { UserId = userB.Id, JoinedAt = DateTime.UtcNow }
            ]
        };
        _context.Users.AddRange(userA, userB, outsider);
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        var result = await _sut.IsParticipantAsync(conversation.Id, outsider.Id);

        Assert.False(result);
    }

    private static User CreateUser()
    {
        var id = Guid.NewGuid();
        return new User
        {
            Id = id,
            DisplayName = "Test User",
            Username = $"user_{id:N}",
            Email = $"{id:N}@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _context.Dispose();
    }
}
