using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Repositories;

public interface IReactionRepository
{
    void Add(Reaction reaction);
    Reaction? GetById(Guid id);
    IReadOnlyList<Reaction> GetAll();
    bool Update(Reaction reactionUpdated);
    Reaction? GetByUserAndTarget(Guid userId, Guid targetId);
    List<Reaction> GetByTargetId(Guid targetId);
    int DeleteByTargetId(Guid targetId);
    void DeleteByTargetIds(List<Guid> ids);
    bool Delete(Guid id);
}
