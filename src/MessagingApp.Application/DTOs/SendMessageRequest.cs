using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Application.DTOs;

public record SendMessageRequest(
    [Required] Guid RecipientId,
    [Required, MinLength(1), MaxLength(2000)] string Content);
