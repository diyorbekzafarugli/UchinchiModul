using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _dbContext;
    public GenericRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }
    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync(int page, int pageSize)
    {
        return await _dbContext.Set<T>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>()
            .FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
