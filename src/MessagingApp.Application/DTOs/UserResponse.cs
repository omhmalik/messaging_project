namespace MessagingApp.Application.DTOs;

public record UserResponse(Guid Id, string DisplayName, string Username, string Email, string? ProfilePicturePath);
