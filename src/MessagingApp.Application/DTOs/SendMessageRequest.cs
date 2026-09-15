using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

/// <summary>A message to send to another user.</summary>
/// <param name="RecipientId">The Id of the user to message. A direct conversation is created automatically if one doesn't exist yet.</param>
/// <param name="Content">The message text.</param>
public record SendMessageRequest(
    [Required] Guid RecipientId,
    [Required, MinLength(1), MaxLength(2000)] string Content);
