using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.DTOs.Users.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<UserResponseDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken);
    Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateAsync(UserUpdateDto dto, CancellationToken cancellationToken);
    Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAccountAsync(CancellationToken cancellationToken);
}