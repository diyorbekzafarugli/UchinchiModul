namespace Order.Api.Repositories;

public interface IRepository<T>
{
    public Guid Add(T entity);
    public bool Update(T entity);
    public bool Delete(Guid id);
    public T? GetById(Guid id);
    public List<T> GetAll();
}
