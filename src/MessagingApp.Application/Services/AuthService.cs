using MessagingApp.Application.DTOs;
using MessagingApp.Application.Exceptions;
using MessagingApp.Application.Interfaces;
using MessagingApp.Domain.Entities;

namespace MessagingApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IFileStorageService _fileStorageService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _fileStorageService = fileStorageService;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _unitOfWork.Users.GetByEmailAsync(request.Email) is not null)
        {
            throw new ConflictException("Email is already registered.");
        }

        if (await _unitOfWork.Users.GetByUsernameAsync(request.Username) is not null)
        {
            throw new ConflictException("Username is already taken.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserResponse(user.Id, user.DisplayName, user.Username, user.Email, user.ProfilePicturePath);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(user);
        var userResponse = new UserResponse(user.Id, user.DisplayName, user.Username, user.Email, user.ProfilePicturePath);

        return new AuthResponse(token, userResponse);
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        return new UserResponse(user.Id, user.DisplayName, user.Username, user.Email, user.ProfilePicturePath);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var existingByEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingByEmail is not null && existingByEmail.Id != userId)
        {
            throw new ConflictException("Email is already registered.");
        }

        var existingByUsername = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (existingByUsername is not null && existingByUsername.Id != userId)
        {
            throw new ConflictException("Username is already taken.");
        }

        user.DisplayName = request.DisplayName;
        user.Username = request.Username;
        user.Email = request.Email;

        await _unitOfWork.SaveChangesAsync();

        return new UserResponse(user.Id, user.DisplayName, user.Username, user.Email, user.ProfilePicturePath);
    }

    public async Task<UserResponse> UpdateProfilePictureAsync(Guid userId, Stream content, string fileExtension)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        user.ProfilePicturePath = await _fileStorageService.SaveProfilePictureAsync(userId, content, fileExtension);

        await _unitOfWork.SaveChangesAsync();

        return new UserResponse(user.Id, user.DisplayName, user.Username, user.Email, user.ProfilePicturePath);
    }
}
