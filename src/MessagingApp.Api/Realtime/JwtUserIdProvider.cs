using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Api.Realtime;

/// <summary>
/// Tells SignalR how to identify "the user" behind a connection, based on the "sub" claim
/// from our JWT (the default provider looks for ClaimTypes.NameIdentifier, which we don't
/// use since MapInboundClaims is disabled). This is what makes Clients.User(userId) work.
/// </summary>
public class JwtUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
