using MessagingApp.Domain.Entities;
using MessagingApp.Infrastructure.Persistence;
using MessagingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace MessagingApp.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class UserRepositoryTests : IDisposable
{
    private readonly MessagingAppDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly UserRepository _sut;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _transaction = _context.Database.BeginTransaction();
        _sut = new UserRepository(_context);
    }

    [Fact]
    public async Task SearchByUsernameAsync_WithPartialAndDifferentCase_StillFindsTheUser()
    {
        var id = Guid.NewGuid();
        var username = $"Sara_Ali_{id:N}";
        var user = new User
        {
            Id = id,
            DisplayName = "Sara Ali",
            Username = username,
            Email = $"{id:N}@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Search with lowercase + only part of the username, to prove EF.Functions.ILike works
        var result = await _sut.SearchByUsernameAsync(username[..8].ToLowerInvariant());

        Assert.Contains(result, u => u.Id == user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WithUnknownEmail_ReturnsNull()
    {
        var result = await _sut.GetByEmailAsync($"{Guid.NewGuid():N}@nowhere.example.com");

        Assert.Null(result);
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _context.Dispose();
    }
}
