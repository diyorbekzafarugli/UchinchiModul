using Microsoft.AspNetCore.Identity;
using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Repositories;

public class UserRepository : JsonRepository<User>, IUserRepository
{

    public UserRepository() : base("Users")
    {
    }

    public User? GetByUserName(string userName)
    {
        return GetAll()
            .FirstOrDefault(u => u.UserName == userName);
    }

    public List<User> GetUsersByIds(List<Guid> ids)
    {
        return GetAll()
            .Where(u => ids.Contains(u.Id))
            .ToList();
    }

    public List<User> GetUsersByName(string searchTerm, int page, int pageSize)
    {
        return GetAll()
            .Where(u => u.UserName.Contains(searchTerm)
            || u.LastName.Contains(searchTerm)
            || u.FirstName.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
