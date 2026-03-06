using StudentCoursePlatform.Application.DTOs.Users;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserResponseDto>> CreateAsync(UserCreateDto userCreateDto);

    Task<Result<UserResponseDto?>> GetByIdAsync(Guid id);
    Task<Result<UserResponseDto?>> GetByEmailAsync(string email);
    Task<Result<List<UserResponseDto>>> GetAllAsync(int page, int pageSize);
    Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<Result<bool>> UpdateAsync(Guid userId, UserUpdateDto userUpdateDto);
    Task<Result<bool>> DeleteAsync(Guid id);
}