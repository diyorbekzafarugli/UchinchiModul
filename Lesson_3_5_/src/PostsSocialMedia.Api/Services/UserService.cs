using AutoMapper;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public Result<bool> Delete(Guid id)
    {
        if (id == Guid.Empty) return Result<bool>.Fail("Foydalanuvchi IDsi xato kiritldi");

        bool isExist = _userRepository.Delete(id);
        if (!isExist) return Result<bool>.Fail("Foydalanuvchi topilmadi");

        return Result<bool>.Ok(isExist);
    }

    public Result<UserGetDto> GetById(Guid id)
    {
        if (id == Guid.Empty) return Result<UserGetDto>.Fail("Foydalanuvchi IDsi xato kiritldi");

        var user = _userRepository.GetById(id);
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi");

        if (user.IsBlocked) return Result<UserGetDto>.Fail("Foydalanuvchi block holatida");

        var userDto = _mapper.Map<UserGetDto>(user);

        return Result<UserGetDto>.Ok(userDto);
    }

    public Result<UserGetDto> GetByUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<UserGetDto>.Fail("Foydalanuvchi nomi bo'sh bo'lishi mumkin emas");

        var user = _userRepository.GetByUserName(AuthService.Normalize(userName));
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi");

        if (user.IsBlocked) return Result<UserGetDto>.Fail("Foydalanuvhci block holatida");

        var userDto = _mapper.Map<UserGetDto>(user);

        return Result<UserGetDto>.Ok(userDto);
    }

    public Result<List<UserGetDto>> Search(string searchTerm, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<UserGetDto>>.Fail("Qidiralayotgan foydalanuvchining nomi bo'sh bo'lishi mumkin emas");

        if (page <= 0 || pageSize <= 0)
            return Result<List<UserGetDto>>.Fail("Sahifa raqami manfiy bo'lishi mumkin emas");

        var users = _userRepository.GetUsersByName(searchTerm, page, pageSize);

        var usersDto = _mapper.Map<List<UserGetDto>>(users);
        return Result<List<UserGetDto>>.Ok(usersDto);
    }

    public Result<bool> Update(Guid currentUserId, UserUpdateDto userUpdateDto)
    {
        if (currentUserId != userUpdateDto.Id)
            return Result<bool>.Fail("Siz boshqa foydalanuvchining profilini tahrirlay olmaysiz!");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi.");

        if (userFromDB.IsBlocked)
            return Result<bool>.Fail("Block holatidasiz!");

        string normalizedNewUserName = AuthService.Normalize(userUpdateDto.UserName);
        if (userFromDB.UserName != normalizedNewUserName)
        {
            var existingUser = _userRepository.GetByUserName(userUpdateDto.UserName);
            if (existingUser is not null)
                return Result<bool>.Fail("Foydalanuvchi nomi band iltimos qaytadan kiriting");
        }

        var validation = ValidateUserUpdate(userUpdateDto);
        if (!validation.Success) return validation;

        _mapper.Map(userUpdateDto, userFromDB);
        userFromDB.UpdatedAt = DateTime.UtcNow;

        return Result<bool>.Ok(_userRepository.Update(userFromDB));
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
