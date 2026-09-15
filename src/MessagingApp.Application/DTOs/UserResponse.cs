namespace MessagingApp.Application.DTOs;

/// <summary>Public profile information for a user. Never includes the password hash.</summary>
/// <param name="Id">The user's permanent identifier. Stable even if the user later changes their name, username, or picture.</param>
/// <param name="DisplayName">The user's regular, changeable display name.</param>
/// <param name="Username">The user's unique, changeable handle.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="ProfilePicturePath">Path to the user's uploaded profile picture, or null if none was set.</param>
public record UserResponse(Guid Id, string DisplayName, string Username, string Email, string? ProfilePicturePath);
