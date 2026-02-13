using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface IReactionRepository
{
    void Add(Reaction reaction);
    Reaction? GetById(Guid id);
    IReadOnlyList<Reaction> GetAll();
    bool Update(Reaction reactionUpdated);
    bool Delete(Guid id);
}
