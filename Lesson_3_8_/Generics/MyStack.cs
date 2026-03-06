namespace Generics;

public class MyStack<T>
{
    private T[] _arr;
    private int _count;
    private readonly static T[] _emptyArray = new T[0];
    private const int DefaultCapacity = 4;
    public MyStack()
    {
        _arr = _emptyArray;
    }

    public MyStack(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        _arr = capacity == 0 ? _emptyArray : new T[capacity];
        _count = 0;
    }
    public int Count => _count;
    public int Capacity => _arr.Length;

    public void Push(T item)
    {
        if (_count == _arr.Length)
            DoubleSize();

        _arr[_count++] = item;
    }

    public T Pop()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty.");

        int idx = --_count;
        T value = _arr[idx];
        _arr[idx] = default;
        return value;
    }

    public T Peek()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty.");

        return _arr[_count - 1];
    }
    private void DoubleSize()
    {
        int newCamp = _arr.Length == 0 ? DefaultCapacity : _arr.Length * 2;
        var newArr = new T[newCamp];
        for (int i = 0; i < _arr.Length; i++)
        {
            newArr[i] = _arr[i];
        }
        _arr = newArr;
    }
}
