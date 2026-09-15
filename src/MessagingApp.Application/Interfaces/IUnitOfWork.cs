namespace MessagingApp.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    Task<int> SaveChangesAsync();
}
