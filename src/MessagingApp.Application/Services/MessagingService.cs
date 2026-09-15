using MessagingApp.Application.DTOs;
using MessagingApp.Application.Exceptions;
using MessagingApp.Application.Interfaces;
using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Services;

public class MessagingService : IMessagingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserResponse>> SearchUsersAsync(Guid currentUserId, string query)
    {
        var users = await _unitOfWork.Users.SearchByUsernameAsync(query);

        return users
            .Where(u => u.Id != currentUserId)
            .Select(u => new UserResponse(u.Id, u.DisplayName, u.Username, u.Email, u.ProfilePicturePath))
            .ToList();
    }

    public async Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        if (senderId == request.RecipientId)
        {
            throw new BadRequestException("You cannot send a message to yourself.");
        }

        if (await _unitOfWork.Users.GetByIdAsync(request.RecipientId) is null)
        {
            throw new NotFoundException("Recipient not found.");
        }

        var conversation = await _unitOfWork.Conversations.GetDirectConversationAsync(senderId, request.RecipientId);

        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
                {
                    new() { UserId = senderId, JoinedAt = DateTime.UtcNow },
                    new() { UserId = request.RecipientId, JoinedAt = DateTime.UtcNow }
                }
            };

            await _unitOfWork.Conversations.AddAsync(conversation);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = senderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return new MessageResponse(message.Id, message.ConversationId, message.SenderId, message.Content, message.SentAt);
    }

    public async Task<IReadOnlyList<ConversationSummaryResponse>> GetConversationsAsync(Guid userId, string? search)
    {
        var conversations = await _unitOfWork.Conversations.GetUserConversationsAsync(userId);

        var summaries = conversations.Select(c =>
        {
            var otherUser = c.Participants.First(p => p.UserId != userId).User;
            var lastMessage = c.Messages.FirstOrDefault();

            return new ConversationSummaryResponse(
                c.Id,
                new UserResponse(otherUser.Id, otherUser.DisplayName, otherUser.Username, otherUser.Email, otherUser.ProfilePicturePath),
                lastMessage?.Content,
                lastMessage?.SentAt);
        });

        if (!string.IsNullOrWhiteSpace(search))
        {
            summaries = summaries.Where(c =>
                c.OtherUser.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return summaries.OrderByDescending(c => c.LastMessageAt).ToList();
    }

    public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid userId, Guid conversationId, DateTime? before, int take)
    {
        if (!await _unitOfWork.Conversations.IsParticipantAsync(conversationId, userId))
        {
            throw new ForbiddenException("You are not a participant of this conversation.");
        }

        var messages = await _unitOfWork.Messages.GetMessagesAsync(conversationId, before, take);

        return messages
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponse(m.Id, m.ConversationId, m.SenderId, m.Content, m.SentAt))
            .ToList();
    }
}
