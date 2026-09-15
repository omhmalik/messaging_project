using MessagingApp.Application.DTOs;
using MessagingApp.Application.Exceptions;
using MessagingApp.Application.Interfaces;
using MessagingApp.Application.Services;
using MessagingApp.Domain.Entities;
using Moq;
using Xunit;

namespace MessagingApp.Application.Tests.Services;

public class MessagingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IConversationRepository> _conversationRepositoryMock = new();
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
    private readonly MessagingService _sut;

    public MessagingServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Conversations).Returns(_conversationRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Messages).Returns(_messageRepositoryMock.Object);
        _sut = new MessagingService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_ToSelf_ThrowsBadRequestException()
    {
        var userId = Guid.NewGuid();
        var request = new SendMessageRequest(userId, "hi");

        await Assert.ThrowsAsync<BadRequestException>(() => _sut.SendMessageAsync(userId, request));
    }

    [Fact]
    public async Task SendMessageAsync_ToNonExistentRecipient_ThrowsNotFoundException()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
        var request = new SendMessageRequest(Guid.NewGuid(), "hi");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.SendMessageAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task SendMessageAsync_WithNoExistingConversation_CreatesNewConversation()
    {
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(recipientId)).ReturnsAsync(new User { Id = recipientId });
        _conversationRepositoryMock
            .Setup(r => r.GetDirectConversationAsync(senderId, recipientId))
            .ReturnsAsync((Conversation?)null);

        var result = await _sut.SendMessageAsync(senderId, new SendMessageRequest(recipientId, "Hello"));

        Assert.Equal("Hello", result.Content);
        _conversationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Conversation>()), Times.Once);
        _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_WithExistingConversation_ReusesItInsteadOfCreatingNew()
    {
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var existingConversation = new Conversation { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(recipientId)).ReturnsAsync(new User { Id = recipientId });
        _conversationRepositoryMock
            .Setup(r => r.GetDirectConversationAsync(senderId, recipientId))
            .ReturnsAsync(existingConversation);

        var result = await _sut.SendMessageAsync(senderId, new SendMessageRequest(recipientId, "Hello again"));

        Assert.Equal(existingConversation.Id, result.ConversationId);
        _conversationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task GetMessagesAsync_WhenUserIsNotParticipant_ThrowsForbiddenException()
    {
        _conversationRepositoryMock
            .Setup(r => r.IsParticipantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.GetMessagesAsync(Guid.NewGuid(), Guid.NewGuid(), null, 20));
    }
}
