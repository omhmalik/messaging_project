namespace MessagingApp.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
