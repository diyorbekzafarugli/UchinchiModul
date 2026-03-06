namespace Generics;

public class MyQueue<T> where T : struct
{
    private T[] arr;
    private int count;
    private readonly static T[] emptyArray = new T[0];
    private const int DefaultCapacity = 4;

    public MyQueue()
    {
        arr = emptyArray;
    }

    public MyQueue(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
        arr = capacity == 0 ? emptyArray : new T[capacity];
        count = 0;
    }

    public int Count => count;
    public int Capacity => arr.Length;

    public void Enqueue(T item)
    {
        if (arr.Length == 0)
            DoubleSize();

        arr[count++] = item;
    }
    public T Dequeue()
    {
        if (count == 0)
            throw new InvalidOperationException("Queue is empty.");

        var res = arr[0];
        for (int i = 0; i < count - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        arr[count - 1] = default;
        count--;

        return res;
    }
    public T Peek()
    {
        return arr[0];
    }
    private void DoubleSize()
    {
        int newCapacity = arr.Length == 0 ? DefaultCapacity : arr.Length * 2;
        T[] newArray = new T[newCapacity];
        for (int i = 0; i < arr.Length; i++)
        {
            newArray[i] = arr[i];
        }

        arr = newArray;
    }
}