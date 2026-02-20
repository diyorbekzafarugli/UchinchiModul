using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface IJsonRepository<T> where T : class, IEntity
{
    Task Add(T item);
    Task<T?> GetById(Guid id);
    Task<IReadOnlyList<T>> GetAll();
    Task Delete(Guid id);
}
