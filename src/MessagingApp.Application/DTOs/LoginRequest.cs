using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

/// <summary>Credentials used to log in.</summary>
/// <param name="Email">The account's email address.</param>
/// <param name="Password">The account's password.</param>
public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
