using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.DTOs.Users.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Domain.Enums;

namespace StudentCoursePlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<UserRegisterDto> _validator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IStringLocalizer<AuthService> _localizer;
    private readonly ILogger<AuthService> _logger;
    public AuthService(IUserRepository userRepository,
                       IJwtProvider jwtProvider,
                       IPasswordHasher passwordHasher,
                       IValidator<UserRegisterDto> validator,
                       IRefreshTokenService refreshTokenService,
                       IStringLocalizer<AuthService> localizer,
                       ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _refreshTokenService = refreshTokenService;
        _localizer = localizer;
        _logger = logger;
    }
    public async Task<Result<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Authenticating user {Email}", userLoginDto.Email);

        if (string.IsNullOrWhiteSpace(userLoginDto.Email)
            || string.IsNullOrWhiteSpace(userLoginDto.Password))
            return Result<LoginResponseDto>.Fail(_localizer["EmailOrPasswordRequired"]);

        var userFromDb = await _userRepository.GetByEmailAsync(userLoginDto.Email, cancellationToken);
        if (userFromDb is null)
        {
            _logger.LogWarning("User not found: {Email}", userLoginDto.Email);
            return Result<LoginResponseDto>.Fail(_localizer["InvalidCredentials"]);
        }

        var isValidPassword = _passwordHasher.Verify(userLoginDto.Password, userFromDb.PasswordHash);
        if (!isValidPassword)
        {
            _logger.LogWarning("Invalid password: {Email}", userLoginDto.Email);
            return Result<LoginResponseDto>.Fail(_localizer["InvalidCredentials"]);
        }

        var response = await BuildLoginResponseAsync(cancellationToken, userFromDb);

        _logger.LogInformation("User {Email} authenticated successfully", userLoginDto.Email);
        return Result<LoginResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto userRegisterDto,
        CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(userRegisterDto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<AuthResponseDto>.Fail(errors);
        }

        _logger.LogInformation("Registering user {Email}", userRegisterDto.Email);

        var existingUser = await _userRepository.GetByEmailAsync(userRegisterDto.Email, cancellationToken);
        if (existingUser is not null)
        {
            _logger.LogWarning("Email already registered: {Email}", userRegisterDto.Email);
            return Result<AuthResponseDto>.Fail(_localizer["UserAlreadyRegistered"]);
        }

        var user = new User
        {
            FullName = userRegisterDto.FullName.Trim(),
            Email = userRegisterDto.Email,
            PasswordHash = _passwordHasher.Hash(userRegisterDto.Password),
            Role = UserRole.Student
        };

        await using var transaction = await _userRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
            var (_, plainToken) = await _refreshTokenService.CreateTokenAsync(user.Id, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("User {Email} registered successfully", user.Email);

            var accessToken = _jwtProvider.GenerateToken(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                User = MapToUserResponse(user),
                AccessToken = accessToken,
                RefreshToken = plainToken,
                AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(_jwtProvider.ExpiryMinutes)
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Registration failed for {Email}", user.Email);
            return Result<AuthResponseDto>.Fail(_localizer["RegistrationFailed"]);
        }
    }

    public async Task<Result<LoginResponseDto>> RefreshAsync(string plainToken,
        CancellationToken cancellationToken)
    {
        var tokenFromDb = await _refreshTokenService.GetTokenAsync(plainToken, cancellationToken);

        if (tokenFromDb is null)
            return Result<LoginResponseDto>.Fail(_localizer["InvalidToken"]);

        if (!tokenFromDb.IsActive)
            return Result<LoginResponseDto>.Fail(_localizer["TokenExpired"]);

        var user = await _userRepository.GetByIdAsync(tokenFromDb.UserId, cancellationToken);
        if (user is null)
            return Result<LoginResponseDto>.Fail(_localizer["UserNotFound"]);

        var (_, newPlainToken) = await _refreshTokenService.CreateTokenAsync(user.Id, cancellationToken);

        tokenFromDb.RevokeAndReplace(newPlainToken);
        await _refreshTokenService.UpdateAsync(tokenFromDb, cancellationToken);

        var response = await BuildLoginResponseAsync(cancellationToken, user, newPlainToken);
        return Result<LoginResponseDto>.Success(response);
    }

    private async Task<LoginResponseDto> BuildLoginResponseAsync(CancellationToken cancellationToken,
        User user, string? plainToken = null)
    {
        var accessToken = _jwtProvider.GenerateToken(user);

        if (plainToken is null)
        {
            var (_, newPlainToken) = await _refreshTokenService.CreateTokenAsync(user.Id, cancellationToken);
            plainToken = newPlainToken;
        }

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = plainToken,
            AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(_jwtProvider.ExpiryMinutes),
            User = MapToUserResponse(user)
        };
    }

    private static UserResponseDto MapToUserResponse(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}
