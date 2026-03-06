namespace Generics.Repositories;

public interface IGenericRepository<T>
{
    void Add(T item);
    bool Remove(Guid id);
    List<T> GetAll();
}
