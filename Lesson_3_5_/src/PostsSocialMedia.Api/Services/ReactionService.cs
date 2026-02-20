using Mapster;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Reaction;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public ReactionService(
        IReactionRepository reactionRepository,
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        _reactionRepository = reactionRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task<Result<Guid>> Add(Guid currentUserId, ReactionAddDto reactionDto)
    {
        if (currentUserId == Guid.Empty)
            return Result<Guid>.Fail("Foydalanuvchi ID si xato");

        if (reactionDto.TargetId == Guid.Empty)
            return Result<Guid>.Fail("Target ID si xato");

        if (!Enum.IsDefined(typeof(ReactionType), reactionDto.Type))
            return Result<Guid>.Fail("Noto'g'ri reaksiya turi");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB == null || userFromDB.IsBlocked)
            return Result<Guid>.Fail("Amalni bajara olmaysiz");

        var existingReaction = await _reactionRepository.GetByUserAndTarget(currentUserId, reactionDto.TargetId);

        if (existingReaction != null)
        {
            if (reactionDto.Type == existingReaction.Type)
            {
                await _reactionRepository.Delete(existingReaction.Id);
                return Result<Guid>.Ok(Guid.Empty);
            }

            existingReaction.Type = reactionDto.Type;
            existingReaction.CreatedAt = DateTime.UtcNow;
            await _reactionRepository.Update(existingReaction);
            return Result<Guid>.Ok(existingReaction.Id);
        }

        var postFromDB = await _postRepository.GetById(reactionDto.TargetId);
        var commentFromDB = await _commentRepository.GetById(reactionDto.TargetId);

        if (postFromDB == null && commentFromDB == null)
            return Result<Guid>.Fail("Reaksiya bildirilayotgan post yoki comment topilmadi");

        var reaction = reactionDto.Adapt<Reaction>();
        reaction.Id = Guid.NewGuid();
        reaction.UserId = currentUserId;
        reaction.CreatedAt = DateTime.UtcNow;

        await _reactionRepository.Add(reaction);
        return Result<Guid>.Ok(reaction.Id);
    }

    public async Task<Result<List<ReactionGetDto>>> GetAll(Guid currentUserId)
    {
        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<List<ReactionGetDto>>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.Role == UserRole.User)
            return Result<List<ReactionGetDto>>.Fail("Amalni bajara olmaysiz");

        var reactions = await _reactionRepository.GetAll();
        if (reactions.Count == 0) return Result<List<ReactionGetDto>>.Ok(new List<ReactionGetDto>());

        var reactionsDto = reactions.Adapt<List<ReactionGetDto>>();
        return Result<List<ReactionGetDto>>.Ok(reactionsDto);
    }

    public async Task<Result<ReactionGetDto>> GetById(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<ReactionGetDto>.Fail("ID kiritishda xatolik");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB == null)
            return Result<ReactionGetDto>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<ReactionGetDto>.Fail("Blocklangansiz");

        var existingReaction = await _reactionRepository.GetById(id);

        if (existingReaction == null
            || (existingReaction.UserId != currentUserId && userFromDB.Role == UserRole.User))
            return Result<ReactionGetDto>.Fail("Siz bundan foydalana olmaysiz");

        return Result<ReactionGetDto>.Ok(existingReaction.Adapt<ReactionGetDto>());
    }

    public async Task<Result<ReactionStatsDto>> GetStatsByTargetId(Guid currentUserId, Guid targetId)
    {
        if (currentUserId == Guid.Empty || targetId == Guid.Empty)
            return Result<ReactionStatsDto>.Fail("ID xato kiritildi");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB == null)
            return Result<ReactionStatsDto>.Fail("Ro'yxatdan o'tmagansiz");

        if (userFromDB.IsBlocked)
            return Result<ReactionStatsDto>.Fail("Blocklangansiz");

        var reactions = await _reactionRepository.GetByTargetId(targetId);

        if (reactions.Count == 0)
        {
            return Result<ReactionStatsDto>.Ok(new ReactionStatsDto
            {
                TargetId = targetId,
                TotalCount = 0,
                Counts = new Dictionary<ReactionType, int>(),
                MyReaction = null
            });
        }

        var statistics = new ReactionStatsDto
        {
            TargetId = targetId,
            TotalCount = reactions.Count,
            Counts = reactions.GroupBy(r => r.Type)
                             .ToDictionary(g => g.Key, g => g.Count()),
            MyReaction = reactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type
        };

        return Result<ReactionStatsDto>.Ok(statistics);
    }
}