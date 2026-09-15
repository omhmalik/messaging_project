using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

/// <summary>Data to update on an existing profile.</summary>
/// <param name="DisplayName">The user's new display name.</param>
/// <param name="Username">The user's new username (must remain unique).</param>
/// <param name="Email">The user's new email address (must remain unique).</param>
public record UpdateProfileRequest(
    [Required, MinLength(2), MaxLength(50)] string DisplayName,
    [Required, MinLength(3), MaxLength(30)] string Username,
    [Required, EmailAddress] string Email);
