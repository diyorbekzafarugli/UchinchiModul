using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Users;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponseDto>> CreateAsync(UserCreateDto userCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<UserResponseDto>>> GetAllAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponseDto?>> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponseDto?>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(Guid userId, UserUpdateDto userUpdateDto)
    {
        throw new NotImplementedException();
    }
}
