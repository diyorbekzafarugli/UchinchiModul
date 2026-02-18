using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Repositories;

public interface IUserRepository
{
    public void Add(User user);
    public User? GetById(Guid id);
    public List<User> GetUsersByIds(List<Guid> ids);
    public bool Update(User userUpdated);
    public bool Delete(Guid id);
    public IReadOnlyList<User> GetAll();
    public User? GetByUserName(string userName);
    public List<User> GetUsersByName(string searchTerm, int page, int pageSize);
}
