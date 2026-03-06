namespace Generics;

public class MyList<T> : IMyList<T>
{
    private T[] _arr;
    private int _arrIndex = 0;
    private int _size = 0;

    public void Add(T item)
    {
        if (_arrIndex >= Capacity)
        {
            DoubleCapacity();
        }
        _arr[_arrIndex] = item;
        _arrIndex++;
    }

    public int Capacity
    {
        get { return _arr.Length; }
    }

    public bool Contains(T item)
    {
        return _arr.Contains(item);
    }

    public T GetById(int index)
    {
        if (index > _size) throw new IndexOutOfRangeException();
        return _arr[index];
    }

    public int IndexOf(T item)
    {
        //if (item is null) throw new Exception();
        for (var i = 0; i < Capacity; i++)
        {
            if (_arr[i]!.Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Remove(T item)
    {

        for (int i = 0; i < _arr.Length; i++)
        {
            if (_arr[i]!.Equals(item))
            {
                for (int j = 0; j < Capacity - 1; j++)
                {
                    _arr[j] = _arr[j + 1];

                }
                _arrIndex--;
                return;
            }
        }
    }

    public bool RemoveAll(T item)
    {
        for (int i = 0; i < _arr.Length; i++)
        {
            if (_arr[i]!.Equals(item))
            {
                for (int j = 0; j < Capacity - 1; j++)
                {
                    _arr[j] = _arr[j + 1];

                }
                _arrIndex--;
                return true;
            }
        }
        return false;

    }

    public void RemoveAt(int index)
    {
        if (index < 0) return;
        for (int i = index; i < _arr.Length - 1; i++)
        {
            _arr[i] = _arr[i + 1];
            _arrIndex--;
            return;
        }
    }

    private void DoubleCapacity()
    {
        T[] newArr = new T[Capacity * 2];

        for (int i = 0; i < Capacity; i++)
        {
            newArr[i] = _arr[i];
        }
        _arr = newArr;
    }
}