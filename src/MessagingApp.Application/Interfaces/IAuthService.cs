using MessagingApp.Application.DTOs;

namespace MessagingApp.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> GetCurrentUserAsync(Guid userId);
    Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<UserResponse> UpdateProfilePictureAsync(Guid userId, Stream content, string fileExtension);
}
