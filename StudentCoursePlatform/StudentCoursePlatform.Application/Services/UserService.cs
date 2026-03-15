using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.DTOs.Users.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace StudentCoursePlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<UserCreateDto> _validator;
    private readonly IValidator<ChangePasswordDto> _validatorPassword;
    private readonly ICurrentUserService _currentUser;
    private readonly IBlacklistService _blacklist;

    public UserService(IUserRepository userRepository,
                       IStringLocalizer<UserService> localizer,
                       ILogger<UserService> logger,
                       IPasswordHasher passwordHasher,
                       IValidator<UserCreateDto> validator,
                       IValidator<ChangePasswordDto> validatorPassword,
                       ICurrentUserService currentUser,
                       IBlacklistService blacklist)
    {
        _userRepository = userRepository;
        _localizer = localizer;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _validatorPassword = validatorPassword;
        _currentUser = currentUser;
        _blacklist = blacklist;
    }
    public async Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        _logger.LogInformation("User {Guid} changing password", userId);
        var validationResult = _validatorPassword.Validate(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<bool>.Fail(errors);
        }

        var userFromDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userFromDb is null)
        {
            _logger.LogWarning("User {Guid} not found", userId);
            return Result<bool>.Fail(_localizer["UserNotFound"]);
        }

        var isSuccess = _passwordHasher.Verify(dto.CurrentPassword, userFromDb.PasswordHash);
        if (!isSuccess)
        {
            _logger.LogWarning("User {Guid} enter incorrect password", userId);
            return Result<bool>.Fail(_localizer["PasswordIncorrect"]);
        }

        var hashedPassword = _passwordHasher.Hash(dto.NewPassword);
        userFromDb.PasswordHash = hashedPassword;

        await _userRepository.UpdateAsync(userFromDb, cancellationToken);
        _logger.LogInformation("User {Guid} changed password successfully", userId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("AdminId is attempting to create user {Email}", dto.Email);

        var validateResult = _validator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join(": ", validateResult.Errors.Select(e => e.ErrorMessage));
            return Result<UserResponseDto>.Fail(errors);
        }

        var userFromDb = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (userFromDb != null)
        {
            _logger.LogWarning("User with this email already registered: {Email}", userFromDb.Email);
            return Result<UserResponseDto>.Fail(_localizer["UserAlreadyRegistered"]);
        }

        _logger.LogInformation("Created user: {Email}", dto.Email);
        var hashedPassword = _passwordHasher.Hash(dto.Password);
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = hashedPassword,
            Role = dto.Role
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return Result<UserResponseDto>.Success(new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<Result<bool>> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        _logger.LogInformation("Deleting userId: {UserId}", userId);

        var userFromDb = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (userFromDb is null)
        {
            _logger.LogWarning("Delete failed. User {UserId} not found", userId);
            return Result<bool>.Fail(_localizer["UserNotFound"]);
        }

        var rawToken = _currentUser.RawToken;
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(rawToken);
        var exspiresAt = jwtToken.ValidTo;

        await _blacklist.AddAsync(rawToken, exspiresAt, cancellationToken);
        await _userRepository.DeleteAsync(userFromDb, cancellationToken);

        _logger.LogInformation("User {UserId} deleted", userId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin is attempting to delete user's {UserId} account", id);
        var userFromDb = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (userFromDb is null)
        {
            _logger.LogWarning("Delete failed. User {UserId} not found", id);
            return Result<bool>.Fail(_localizer["UserNotFound"]);
        }

        await _userRepository.DeleteAsync(userFromDb, cancellationToken);

        _logger.LogInformation("User {UserId} deleted", id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<UserResponseDto>>> GetAllAsync(PaginationParams pagination,
        CancellationToken cancellationToken)
    {

        var users = await _userRepository.GetAllAsync(pagination.Page, pagination.PageSize,
            cancellationToken);
        var result = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }).ToList();

        return Result<List<UserResponseDto>>.Success(result);
    }

    public async Task<Result<UserResponseDto>> GetByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return Result<UserResponseDto>.Fail(_localizer["GuidEmpty"]);

        var userFromDb = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (userFromDb is null)
            return Result<UserResponseDto>.Fail(_localizer["UserNotFound"]);

        return Result<UserResponseDto>.Success(new UserResponseDto
        {
            Id = userFromDb.Id,
            FullName = userFromDb.FullName,
            Email = userFromDb.Email,
            Role = userFromDb.Role,
            CreatedAt = userFromDb.CreatedAt
        });
    }

    public async Task<Result<bool>> UpdateAsync(UserUpdateDto userUpdateDto,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        _logger.LogInformation("User {UserId} is attempting to update their account.", userId);

        var userFromDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userFromDb is null)
        {
            _logger.LogWarning("Update failed. User {UserId} not found", userId);
            return Result<bool>.Fail(_localizer["UserNotFound"]);
        }

        if (string.IsNullOrWhiteSpace(userUpdateDto.FullName))
            return Result<bool>.Fail(_localizer["FullNameEmpty"]);

        userFromDb.FullName = userUpdateDto.FullName;
        userFromDb.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(userFromDb, cancellationToken);

        _logger.LogInformation("User {UserId} updated their account", userId);

        return Result<bool>.Success(true);
    }
}
