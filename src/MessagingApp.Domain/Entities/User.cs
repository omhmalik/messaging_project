namespace MessagingApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? ProfilePicturePath { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ConversationParticipant> ConversationParticipations { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
}
