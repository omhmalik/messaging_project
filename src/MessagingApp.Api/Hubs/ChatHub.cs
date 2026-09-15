using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Api.Hubs;

/// <summary>
/// Real-time channel clients connect to in order to receive new-message notifications.
/// Clients don't call any methods on this hub directly — messages are still sent via
/// the REST API; this hub only pushes notifications out.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
}
