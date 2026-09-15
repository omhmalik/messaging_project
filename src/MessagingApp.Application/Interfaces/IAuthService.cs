using MessagingApp.Application.DTOs;

namespace MessagingApp.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> GetCurrentUserAsync(Guid userId);
}
