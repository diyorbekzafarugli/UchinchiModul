using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

   
}
