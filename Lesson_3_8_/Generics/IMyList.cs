namespace Generics;

public interface IMyList<T>
{
    void Add(T item);
    bool Contains(T item);
    T GetById(int index);
    int IndexOf(T item);
    void Remove(T item);
    bool RemoveAll(T item);
    void RemoveAt(int index);
}