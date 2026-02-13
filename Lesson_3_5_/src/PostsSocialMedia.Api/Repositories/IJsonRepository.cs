using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface IJsonRepository<T> where T : class, IEntity
{
    void Add(T item);
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    bool Update(T updatedItem);
    bool Delete(Guid id);
}
