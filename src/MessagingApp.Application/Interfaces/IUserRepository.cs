using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IReadOnlyList<User>> SearchByUsernameAsync(string usernameQuery);
    Task AddAsync(User user);
}
