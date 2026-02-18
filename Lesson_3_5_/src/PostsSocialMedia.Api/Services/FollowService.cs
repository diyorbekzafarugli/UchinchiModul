using PostsSocialMedia.Api.Dtos.FollowDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Follow;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    public FollowService(IFollowRepository followRepository, IUserRepository userRepository)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
    }
    public Result<Guid> Add(Guid currentUserId, FollowAddDto followAddDto)
    {
        if (currentUserId == Guid.Empty || followAddDto.FollowingId == Guid.Empty)
            return Result<Guid>.Fail("Foydalanuvchi ID si xato");

        var userFollowerFromDB = _userRepository.GetById(currentUserId);
        if (userFollowerFromDB is null)
            return Result<Guid>.Fail("Ro'yxatdan o'tmagansiz");

        if(currentUserId == followAddDto.FollowingId)
            return Result<Guid>.Fail("O'zingizga obuna bo'la olmaysiz");

        bool isFollowed = _followRepository.IsFollowing(currentUserId, followAddDto.FollowingId);
        if (!isFollowed) return Result<Guid>.Fail("Allaqachon obuna bo'lgansiz");

        if (userFollowerFromDB.Id != followAddDto.FollowerId)
            return Result<Guid>.Fail("Foydalanuvchi ma'lumoti xato kiritldi, iltimos qaydatdan kiriting");

        if (userFollowerFromDB.IsBlocked)
            return Result<Guid>.Fail("Blocklangansiz");

        var userFollowingFromDB = _userRepository.GetById(followAddDto.FollowingId);
        if (userFollowingFromDB is null)
            return Result<Guid>.Fail("Siz obuna bo'lmoqchi bo'lgan foydalanuvchi topilmadi");

        if (userFollowingFromDB.IsBlocked)
            return Result<Guid>.Fail("Siz obuna bo'lmoqchi bo'lgan foydalanuvchi block holatida");

        Follow follow = new()
        {
            Id = Guid.NewGuid(),
            FollowerId = followAddDto.FollowerId,
            FollowingId = followAddDto.FollowingId,
            FollowedAt = DateTime.UtcNow
        };

        _followRepository.Add(follow);
        return Result<Guid>.Ok(follow.Id);
    }

    public Result<bool> Delete(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty)
            return Result<bool>.Fail("Foydalanuvchi ID si xato");

        if (id == Guid.Empty)
            return Result<bool>.Fail("Obuna ID si xato kiritildi");

        var userFollowerFromDB = _userRepository.GetById(currentUserId);
        if (userFollowerFromDB is null)
            return Result<bool>.Fail("Ro'yxatdan o'tmagansiz");

        var followFromDB = _followRepository.GetById(id);
        if (followFromDB is null)
            return Result<bool>.Fail("Obuna bo'lganligingiz haqida qayd topilmadi");

        if (followFromDB.FollowerId != userFollowerFromDB.Id)
            return Result<bool>.Fail("Siz faqat o'zingiz obuna bo'lganlardan obunani o'chira olasiz");

        var result = _followRepository.Delete(id);
        if (!result) return Result<bool>.Fail("Iltimos ma'lumotlarni tekshirib qaytadan urinib ko'ring");

        return Result<bool>.Ok(result);
    }

    public Result<List<FollowGetDto>> GetAll(Guid currentUserId)
    {
        if (currentUserId == Guid.Empty)
            return Result<List<FollowGetDto>>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<List<FollowGetDto>>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<List<FollowGetDto>>.Fail("Blocklangansiz");

        var listFollowers = _followRepository.GetAllByUser(userFromDB.Id);
        if (listFollowers is null || !listFollowers.Any())
            return Result<List<FollowGetDto>>.Ok(new List<FollowGetDto>());

        var followersIds = listFollowers.Select(f => f.FollowerId).ToList();
        var followersFromDb = _userRepository.GetUsersByIds(followersIds);
        var myFollowings = _followRepository.GetAllFollowings(userFromDB.Id)
                                            .Select(f => f.FollowingId)
                                            .ToHashSet();

        var userFollowersDto = followersFromDb.Select(user => new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = listFollowers.FirstOrDefault(f => f.FollowerId == user.Id)?.FollowedAt ?? DateTime.MinValue,
            IsFollowedByMe = myFollowings.Contains(user.Id)
        }).ToList();

        return Result<List<FollowGetDto>>.Ok(userFollowersDto);
    }

    public Result<FollowGetDto> GetById(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty)
            return Result<FollowGetDto>.Fail("Foydalanuvchi ID si xato");

        if (id == Guid.Empty)
            return Result<FollowGetDto>.Fail("Obuna ID si xato kiritildi");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<FollowGetDto>.Fail("Ro'yxatdan o'tmagansiz");

        var followFromDB = _followRepository.GetById(id);
        if (followFromDB is null)
            return Result<FollowGetDto>.Fail("Obuna bo'lganligingiz haqida qayd topilmadi");

        var user = _userRepository.GetById(followFromDB.FollowerId);
        if (user is null) return Result<FollowGetDto>.Fail("Foydalanuvchi topilmadi.");

        var userFollowerDto = new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = followFromDB.FollowedAt,
            IsFollowedByMe = _followRepository.IsFollowing(userFromDB.Id, user.Id)
        };

        return Result<FollowGetDto>.Ok(userFollowerDto);
    }

    public Result<List<FollowGetDto>> GetUsersByName(Guid currentUserId, string searchTerm, int page, int pageSize)
    {
        if (currentUserId == Guid.Empty)
            return Result<List<FollowGetDto>>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<List<FollowGetDto>>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<List<FollowGetDto>>.Fail("Blocklangansiz");

        var usersFromDB = _userRepository.GetUsersByName(searchTerm, page, pageSize);
        if(!usersFromDB.Any())
        {
            return Result<List<FollowGetDto>>.Ok(new List<FollowGetDto>());
        }

        var listFollowers = _followRepository.GetAllByUser(currentUserId);

        var myFollowersDict = listFollowers.ToDictionary(k => k.FollowerId, v => v.FollowedAt);

        var myFollowings = _followRepository.GetAllFollowings(currentUserId)
                                            .Select(f => f.FollowingId)
                                            .ToHashSet();

        var userFollowersDto = usersFromDB.Select(user => new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = myFollowersDict.ContainsKey(user.Id) ? myFollowersDict[user.Id] : DateTime.MinValue,
            IsFollowedByMe = myFollowings.Contains(user.Id)
        }).ToList();

        return Result<List<FollowGetDto>>.Ok(userFollowersDto);
    }
}
