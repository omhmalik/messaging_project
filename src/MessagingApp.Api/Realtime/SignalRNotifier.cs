using MessagingApp.Api.Hubs;
using MessagingApp.Application.DTOs;
using MessagingApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Api.Realtime;

public class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyNewMessageAsync(Guid recipientUserId, MessageResponse message)
    {
        return _hubContext.Clients
            .User(recipientUserId.ToString())
            .SendAsync("ReceiveMessage", message);
    }
}
