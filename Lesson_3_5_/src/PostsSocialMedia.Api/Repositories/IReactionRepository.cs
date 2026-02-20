using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Repositories;

public interface IReactionRepository
{
    Task Add(Reaction reaction);
    Task<Reaction?> GetById(Guid id);
    Task<IReadOnlyList<Reaction>> GetAll();
    Task Update(Reaction reactionUpdated);
    Task<Reaction?> GetByUserAndTarget(Guid userId, Guid targetId);
    Task<List<Reaction>> GetByTargetId(Guid targetId);
    Task<int> DeleteByTargetId(Guid targetId);
    Task DeleteByTargetIds(List<Guid> ids);
    Task Delete(Guid id);
}
