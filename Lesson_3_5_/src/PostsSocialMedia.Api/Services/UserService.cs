using Mapster;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty) return Result<bool>.Fail("Foydalanuvchi IDsi xato kiritildi");

        await _userRepository.Delete(id);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<UserGetDto>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty) return Result<UserGetDto>.Fail("Foydalanuvchi IDsi xato kiritildi");

        var user = await _userRepository.GetById(id);
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi");

        if (user.IsBlocked) return Result<UserGetDto>.Fail("Foydalanuvchi block holatida");

        var userDto = user.Adapt<UserGetDto>();
        return Result<UserGetDto>.Ok(userDto);
    }

    public async Task<Result<UserGetDto>> GetByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<UserGetDto>.Fail("Foydalanuvchi nomi bo'sh bo'lishi mumkin emas");

        var user = await _userRepository.GetByUserName(AuthService.Normalize(userName));
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi");

        if (user.IsBlocked) return Result<UserGetDto>.Fail("Foydalanuvchi block holatida");

        var userDto = user.Adapt<UserGetDto>();
        return Result<UserGetDto>.Ok(userDto);
    }

    public async Task<Result<List<UserGetDto>>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<UserGetDto>>.Fail("Qidiralayotgan foydalanuvchining nomi bo'sh bo'lishi mumkin emas");

        if (page <= 0 || pageSize <= 0)
            return Result<List<UserGetDto>>.Fail("Sahifa raqami yoki hajmi xato kiritildi");

        var users = await _userRepository.GetUsersByName(searchTerm, page, pageSize);

        var usersDto = users.Adapt<List<UserGetDto>>();
        return Result<List<UserGetDto>>.Ok(usersDto);
    }

    public async Task<Result<bool>> UpdateAsync(Guid currentUserId, UserUpdateDto userUpdateDto)
    {
        if (currentUserId != userUpdateDto.Id)
            return Result<bool>.Fail("Siz boshqa foydalanuvchining profilini tahrirlay olmaysiz!");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi.");

        if (userFromDB.IsBlocked)
            return Result<bool>.Fail("Block holatidasiz!");

        string normalizedNewUserName = AuthService.Normalize(userUpdateDto.UserName);
        if (userFromDB.UserName != normalizedNewUserName)
        {
            var existingUser = await _userRepository.GetByUserName(userUpdateDto.UserName);
            if (existingUser is not null)
                return Result<bool>.Fail("Foydalanuvchi nomi band iltimos qaytadan kiriting");
        }

        var validation = ValidateUserUpdate(userUpdateDto);
        if (!validation.Success) return validation;

        userUpdateDto.Adapt(userFromDB);
        userFromDB.UpdatedAt = DateTime.UtcNow;

        await _userRepository.Update(userFromDB);
        return Result<bool>.Ok(true);
    }

    private Result<bool> ValidateUserUpdate(UserUpdateDto userUpdateDto)
    {
        if (!IsValidateString(userUpdateDto.UserName))
            return Result<bool>.Fail("Foydalanuvchining nomi bo'sh bo'lmasligi va 64 ta belgidan oshmasligi kerak");

        if (!IsValidateString(userUpdateDto.FirstName))
            return Result<bool>.Fail("Foydalanuvchining ismi bo'sh bo'lmasligi va 64 ta belgidan oshmasligi kerak");

        if (!IsValidateString(userUpdateDto.LastName))
            return Result<bool>.Fail("Foydalanuvchining familiyasi bo'sh bo'lmasligi va 64 ta belgidan oshmasligi kerak");

        if (userUpdateDto.DateOfBirth.AddYears(14) > DateTime.UtcNow)
            return Result<bool>.Fail("Foydalanuvchi 14 yoshdan oshgan bo'lishi kerak");

        return Result<bool>.Ok(true);
    }

    private static bool IsValidateString(string value)
        => !string.IsNullOrWhiteSpace(value) && value.Length <= 64;
}