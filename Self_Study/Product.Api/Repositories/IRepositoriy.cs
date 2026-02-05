namespace Product.Api.Repositories;

public interface IRepositoriy<T>
{
    Guid Add(T entity);
    bool Update(T entity);
    bool Delete(Guid id);
    T? GetById(Guid id);
    List<T> GetAll();
}
