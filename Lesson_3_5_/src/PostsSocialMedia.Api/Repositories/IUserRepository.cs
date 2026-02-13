using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface IUserRepository
{
    void Add(User user);
    User? GetById(Guid id);
    bool Update(User userUpdated);
    bool Delete(Guid id);
    IReadOnlyList<User>? GetAll();
    User? GetByUserName(string userName);
}
