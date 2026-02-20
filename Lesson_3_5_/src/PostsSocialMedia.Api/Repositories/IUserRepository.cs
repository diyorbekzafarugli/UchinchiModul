using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Repositories;

public interface IUserRepository
{
    public Task Add(User user);
    public Task<User?> GetById(Guid id);
    public Task<List<User>> GetUsersByIds(List<Guid> ids);
    public Task Update(User userUpdated);
    public Task Delete(Guid id);
    public Task<IReadOnlyList<User>> GetAll();
    public Task<User?> GetByUserName(string userName);
    public Task<List<User>> GetUsersByName(string searchTerm, int page, int pageSize);
}
