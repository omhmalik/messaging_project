namespace MessagingApp.Application.DTOs;

public record RegisterRequest(string DisplayName, string Username, string Email, string Password);
