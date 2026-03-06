using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IGenericRepository<T> where T : class, IEntity
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync(int page, int pageSize);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
