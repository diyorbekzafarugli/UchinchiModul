using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface IFollowRepository
{
    void Add(Follow follow);
    Follow? GetById(Guid id);
    IReadOnlyList<Follow> GetAll();
    bool Update(Follow followUpdated);
    bool Delete(Guid id);
}
