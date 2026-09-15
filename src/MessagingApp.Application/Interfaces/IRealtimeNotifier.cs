using MessagingApp.Application.DTOs;

namespace MessagingApp.Application.Interfaces;

public interface IRealtimeNotifier
{
    Task NotifyNewMessageAsync(Guid recipientUserId, MessageResponse message);
}
