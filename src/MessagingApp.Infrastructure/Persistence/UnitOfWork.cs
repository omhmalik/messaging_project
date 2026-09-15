using MessagingApp.Application.Interfaces;
using MessagingApp.Infrastructure.Persistence.Repositories;

namespace MessagingApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MessagingAppDbContext _context;

    public UnitOfWork(MessagingAppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
    }

    public IUserRepository Users { get; }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
