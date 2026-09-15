using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
