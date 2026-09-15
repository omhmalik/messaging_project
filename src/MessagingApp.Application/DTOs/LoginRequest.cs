using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
