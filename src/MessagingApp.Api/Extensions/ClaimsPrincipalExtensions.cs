using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MessagingApp.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
    }
}
