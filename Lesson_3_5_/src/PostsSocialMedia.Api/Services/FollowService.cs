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

    public async Task<Result<Guid>> Add(Guid currentUserId, FollowAddDto followAddDto)
    {
        if (currentUserId == Guid.Empty || followAddDto.FollowingId == Guid.Empty)
            return Result<Guid>.Fail("Foydalanuvchi ID si xato");

        var userFollowerFromDB = await _userRepository.GetById(currentUserId);
        if (userFollowerFromDB is null)
            return Result<Guid>.Fail("Ro'yxatdan o'tmagansiz");

        if (currentUserId == followAddDto.FollowingId)
            return Result<Guid>.Fail("O'zingizga obuna bo'la olmaysiz");

        bool isAlreadyFollowing = await _followRepository.IsFollowing(currentUserId, followAddDto.FollowingId);
        if (isAlreadyFollowing)
            return Result<Guid>.Fail("Allaqachon obuna bo'lgansiz");

        if (userFollowerFromDB.Id != followAddDto.FollowerId)
            return Result<Guid>.Fail("Foydalanuvchi ma'lumoti xato kiritildi, iltimos qaytadan kiriting");

        if (userFollowerFromDB.IsBlocked)
            return Result<Guid>.Fail("Blocklangansiz");

        var userFollowingFromDB = await _userRepository.GetById(followAddDto.FollowingId);
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

        await _followRepository.Add(follow);
        return Result<Guid>.Ok(follow.Id);
    }

    public async Task<Result<bool>> Delete(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<bool>.Fail("ID xato kiritildi");

        var userFollowerFromDB = await _userRepository.GetById(currentUserId);
        if (userFollowerFromDB is null)
            return Result<bool>.Fail("Ro'yxatdan o'tmagansiz");

        var followFromDB = await _followRepository.GetById(id);
        if (followFromDB is null)
            return Result<bool>.Fail("Obuna bo'lganligingiz haqida qayd topilmadi");

        if (followFromDB.FollowerId != userFollowerFromDB.Id)
            return Result<bool>.Fail("Siz faqat o'zingiz obuna bo'lganlardan obunani o'chira olasiz");

        await _followRepository.Delete(id);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<FollowGetDto>>> GetAll(Guid currentUserId)
    {
        if (currentUserId == Guid.Empty)
            return Result<List<FollowGetDto>>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<List<FollowGetDto>>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<List<FollowGetDto>>.Fail("Blocklangansiz");

        var listFollowers = await _followRepository.GetAllByUser(userFromDB.Id);
        if (listFollowers is null || !listFollowers.Any())
            return Result<List<FollowGetDto>>.Ok(new List<FollowGetDto>());

        var followersIds = listFollowers.Select(f => f.FollowerId).ToList();
        var followersFromDb = await _userRepository.GetUsersByIds(followersIds);

        var myFollowingList = await _followRepository.GetAllFollowings(userFromDB.Id);
        var myFollowingsSet = myFollowingList.Select(f => f.FollowingId).ToHashSet();

        var userFollowersDto = followersFromDb.Select(user => new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = listFollowers.FirstOrDefault(f => f.FollowerId == user.Id)?.FollowedAt ?? DateTime.MinValue,
            IsFollowedByMe = myFollowingsSet.Contains(user.Id)
        }).ToList();

        return Result<List<FollowGetDto>>.Ok(userFollowersDto);
    }

    public async Task<Result<FollowGetDto>> GetById(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<FollowGetDto>.Fail("ID xato kiritildi");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<FollowGetDto>.Fail("Ro'yxatdan o'tmagansiz");

        var followFromDB = await _followRepository.GetById(id);
        if (followFromDB is null)
            return Result<FollowGetDto>.Fail("Obuna qaydi topilmadi");

        var user = await _userRepository.GetById(followFromDB.FollowerId);
        if (user is null) return Result<FollowGetDto>.Fail("Foydalanuvchi topilmadi.");

        var userFollowerDto = new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = followFromDB.FollowedAt,
            IsFollowedByMe = await _followRepository.IsFollowing(userFromDB.Id, user.Id)
        };

        return Result<FollowGetDto>.Ok(userFollowerDto);
    }

    public async Task<Result<List<FollowGetDto>>> GetUsersByName(Guid currentUserId, string searchTerm, int page, int pageSize)
    {
        if (currentUserId == Guid.Empty)
            return Result<List<FollowGetDto>>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<List<FollowGetDto>>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<List<FollowGetDto>>.Fail("Blocklangansiz");

        var usersFromDB = await _userRepository.GetUsersByName(searchTerm, page, pageSize);
        if (!usersFromDB.Any())
            return Result<List<FollowGetDto>>.Ok(new List<FollowGetDto>());

        var listFollowers = await _followRepository.GetAllByUser(currentUserId);
        var myFollowersDict = listFollowers.ToDictionary(k => k.FollowerId, v => v.FollowedAt);

        var myFollowingList = await _followRepository.GetAllFollowings(currentUserId);
        var myFollowingsSet = myFollowingList.Select(f => f.FollowingId).ToHashSet();

        var userFollowersDto = usersFromDB.Select(user => new FollowGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FollowedAt = myFollowersDict.ContainsKey(user.Id) ? myFollowersDict[user.Id] : DateTime.MinValue,
            IsFollowedByMe = myFollowingsSet.Contains(user.Id)
        }).ToList();

        return Result<List<FollowGetDto>>.Ok(userFollowersDto);
    }
}