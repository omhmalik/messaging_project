using MessagingApp.Domain.Entities;
using MessagingApp.Infrastructure.Persistence;
using MessagingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace MessagingApp.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class MessageRepositoryTests : IDisposable
{
    private readonly MessagingAppDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly MessageRepository _sut;

    public MessageRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _transaction = _context.Database.BeginTransaction();
        _sut = new MessageRepository(_context);
    }

    [Fact]
    public async Task GetMessagesAsync_WithBeforeCursor_ReturnsOnlyOlderMessages()
    {
        var conversationId = Guid.NewGuid();
        var sender = CreateUser();
        _context.Users.Add(sender);
        _context.Conversations.Add(new Conversation { Id = conversationId, CreatedAt = DateTime.UtcNow });

        var baseline = DateTime.UtcNow;
        _context.Messages.AddRange(
            NewMessage(conversationId, sender.Id, "old", baseline.AddMinutes(-10)),
            NewMessage(conversationId, sender.Id, "new", baseline));
        await _context.SaveChangesAsync();

        var result = await _sut.GetMessagesAsync(conversationId, before: baseline, take: 10);

        Assert.Single(result);
        Assert.Equal("old", result[0].Content);
    }

    [Fact]
    public async Task GetMessagesAsync_RespectsTakeLimit()
    {
        var conversationId = Guid.NewGuid();
        var sender = CreateUser();
        _context.Users.Add(sender);
        _context.Conversations.Add(new Conversation { Id = conversationId, CreatedAt = DateTime.UtcNow });

        for (var i = 0; i < 5; i++)
        {
            _context.Messages.Add(NewMessage(conversationId, sender.Id, $"msg-{i}", DateTime.UtcNow.AddMinutes(-i)));
        }
        await _context.SaveChangesAsync();

        var result = await _sut.GetMessagesAsync(conversationId, before: null, take: 2);

        Assert.Equal(2, result.Count);
    }

    private static User CreateUser()
    {
        var id = Guid.NewGuid();
        return new User
        {
            Id = id,
            DisplayName = "Sender",
            Username = $"user_{id:N}",
            Email = $"{id:N}@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Message NewMessage(Guid conversationId, Guid senderId, string content, DateTime sentAt) => new()
    {
        Id = Guid.NewGuid(),
        ConversationId = conversationId,
        SenderId = senderId,
        Content = content,
        SentAt = sentAt
    };

    public void Dispose()
    {
        _transaction.Dispose();
        _context.Dispose();
    }
}
