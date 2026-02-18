using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IReactionService
{
    Result<Guid> Add(Guid currentUserId, ReactionAddDto reaction);
    Result<ReactionGetDto>? GetById(Guid currentUserId, Guid id);
    Result<List<ReactionGetDto>> GetAll(Guid currentUserId);
    Result<ReactionStatsDto> GetStatsByTargetId(Guid currentId, Guid targetId);
}
