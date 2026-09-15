namespace MessagingApp.Application.DTOs;

/// <summary>Result of a successful login.</summary>
/// <param name="Token">A JWT access token. Send it as "Authorization: Bearer &lt;token&gt;" on subsequent requests.</param>
/// <param name="User">The authenticated user's profile.</param>
public record AuthResponse(string Token, UserResponse User);
