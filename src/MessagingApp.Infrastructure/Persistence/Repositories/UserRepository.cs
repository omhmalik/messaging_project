using MessagingApp.Application.Interfaces;
using MessagingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MessagingAppDbContext _context;

    public UserRepository(MessagingAppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByUsernameAsync(string username) =>
        _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<IReadOnlyList<User>> SearchByUsernameAsync(string usernameQuery)
    {
        return await _context.Users
            .Where(u => EF.Functions.ILike(u.Username, $"%{usernameQuery}%"))
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
