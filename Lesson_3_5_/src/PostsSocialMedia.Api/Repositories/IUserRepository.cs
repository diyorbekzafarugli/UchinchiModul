using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Repositories;

public interface IUserRepository
{
    public void Add(User user);
    public User? GetById(Guid id);
    public bool Update(User userUpdated);
    public bool Delete(Guid id);
    public IReadOnlyList<User> GetAll();
    public User? GetByUserName(string userName);
}
