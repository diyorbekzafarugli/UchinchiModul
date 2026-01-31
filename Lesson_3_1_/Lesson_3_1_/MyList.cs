namespace Lesson_3_1_;

public class MyList : IMyList
{
    private int[] _nums;
    private int arrIndex = 0;
    public int Capacity
    {
        get { return _nums.Length; }
    }

    public MyList(int capacity = 4)
    {
        _nums = new int[capacity];
    }
    public bool Add(int num)
    {
        if (arrIndex >= Capacity)
        {
            DoubleCapacity();
        }

        _nums[arrIndex] = num;
        arrIndex++;

        return true;
    }

    private void DoubleCapacity()
    {
        int[] newArr = new int[Capacity * 2];
        for (var i = 0; i < Capacity; i++)
        {
            newArr[i] = _nums[i];
        }
        _nums = newArr;
    }

    public bool Contains(int num)
    {
        throw new NotImplementedException();
    }

    public int GetById(int index)
    {
        return _nums[index];
    }

    public int IndexOf(int num)
    {
        for (var i = 0; i < Capacity; i++)
        {
            if (_nums[i] == num)
            {
                return i;
            }
        }

        return -1;
    }

    public bool Remove(int num)
    {
        for (var i = 0; i < Capacity; i++)
        {
            if (_nums[i] == num)
            {
                for (var j = i; j < Capacity - 1; j++)
                {
                    _nums[j] = _nums[j + 1];
                }
                --arrIndex;
                return true;
            }
        }

        return false;
    }

    public bool RemoveAll(int num)
    {
        var count = 0;

        for (int i = 0; i < Capacity - 1; i++)
        {
            if (_nums[i] == num && _nums[i + 1] != num)
            {
                for (var j = i; j < Capacity - 1; j++)
                {
                    _nums[j] = _nums[j + 1];
                }
                --arrIndex;
                ++count;
            }
            else
            {

            }
        }

        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveAt(int index)
    {
        if (index < Capacity)
        {
            for (var j = index; j < Capacity - 1; j++)
            {
                _nums[j] = _nums[j + 1];
            }
            --arrIndex;
            return true;

        }
        return false;
    }

    public void DisplayElements()
    {
        foreach (var i in _nums)
        {
            Console.Write(i + " ");
        }
    }
}