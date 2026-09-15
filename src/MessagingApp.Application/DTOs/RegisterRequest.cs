using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

/// <summary>Data required to create a new user account.</summary>
/// <param name="DisplayName">The user's regular, changeable display name.</param>
/// <param name="Username">A unique handle other users search by to start a conversation.</param>
/// <param name="Email">A unique email address, used as the login credential.</param>
/// <param name="Password">The account password (will be hashed, never stored as-is).</param>
public record RegisterRequest(
    [Required, MinLength(2), MaxLength(50)] string DisplayName,
    [Required, MinLength(3), MaxLength(30)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password);
