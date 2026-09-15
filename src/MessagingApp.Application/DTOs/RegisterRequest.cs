using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

public record RegisterRequest(
    [Required, MinLength(2), MaxLength(50)] string DisplayName,
    [Required, MinLength(3), MaxLength(30)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password);
