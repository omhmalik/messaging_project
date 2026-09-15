using MessagingApp.Application.DTOs;
using MessagingApp.Application.Exceptions;
using MessagingApp.Application.Interfaces;
using MessagingApp.Application.Services;
using MessagingApp.Domain.Entities;
using Moq;
using Xunit;

namespace MessagingApp.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _sut = new AuthService(_unitOfWorkMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmailAndUsername_CreatesUserWithHashedPassword()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Hash("P@ssw0rd1")).Returns("hashed-password");

        var request = new RegisterRequest("Mohammed", "mohammed_dev", "mohammed@example.com", "P@ssw0rd1");

        var result = await _sut.RegisterAsync(request);

        Assert.Equal("mohammed_dev", result.Username);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.PasswordHash == "hashed-password")), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithAlreadyRegisteredEmail_ThrowsConflictException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var request = new RegisterRequest("Mohammed", "mohammed_dev", "taken@example.com", "P@ssw0rd1");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WithAlreadyTakenUsername_ThrowsConflictException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var request = new RegisterRequest("Mohammed", "taken_username", "new@example.com", "P@ssw0rd1");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsTokenAndUserInfo()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "mohammed@example.com",
            Username = "mohammed_dev",
            PasswordHash = "hashed"
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(user.PasswordHash, "correct-password")).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("fake-jwt-token");

        var result = await _sut.LoginAsync(new LoginRequest(user.Email, "correct-password"));

        Assert.Equal("fake-jwt-token", result.Token);
        Assert.Equal(user.Username, result.User.Username);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsInvalidCredentialsException()
    {
        var user = new User { Email = "mohammed@example.com", PasswordHash = "hashed" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(user.PasswordHash, It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _sut.LoginAsync(new LoginRequest(user.Email, "wrong-password")));
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ThrowsInvalidCredentialsException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _sut.LoginAsync(new LoginRequest("noone@example.com", "whatever")));
    }
}
