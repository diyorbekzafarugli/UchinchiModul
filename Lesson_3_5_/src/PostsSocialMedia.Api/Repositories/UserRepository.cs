using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Repositories;

public class UserRepository : JsonRepository<User>, IUserRepository
{

    public UserRepository() : base("Users") { }

    public async Task<User?> GetByUserName(string userName)
    {
        var users = await GetAll();
        return users.FirstOrDefault(u => u.UserName == userName);
    }

    public async Task<List<User>> GetUsersByIds(List<Guid> ids)
    {
        var users = await GetAll();
        return users.Where(u => ids.Contains(u.Id))
                    .ToList();
    }

    public async Task<List<User>> GetUsersByName(string searchTerm, int page, int pageSize)
    {
        var users = await GetAll();
        return users.Where(u => u.UserName.Contains(searchTerm)
            || u.LastName.Contains(searchTerm)
            || u.FirstName.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
