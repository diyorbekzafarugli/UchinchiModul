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
}
