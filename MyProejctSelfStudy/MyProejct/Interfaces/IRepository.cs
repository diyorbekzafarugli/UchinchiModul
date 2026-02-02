namespace MyProejct.Repositories;

public interface IRepository<T>
{
    Guid Add(T entity);
    List<T> GetAll();
    T? GetById(Guid id);
    bool Update(T entity);
    bool Delete(Guid id);
}
