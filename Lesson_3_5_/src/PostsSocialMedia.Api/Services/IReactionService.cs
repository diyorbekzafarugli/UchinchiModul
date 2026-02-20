using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IReactionService
{

    Task<Result<Guid>> Add(Guid currentUserId, ReactionAddDto reaction);

    Task<Result<ReactionGetDto>> GetById(Guid currentUserId, Guid id);

    Task<Result<List<ReactionGetDto>>> GetAll(Guid currentUserId);

    Task<Result<ReactionStatsDto>> GetStatsByTargetId(Guid currentId, Guid targetId);
}