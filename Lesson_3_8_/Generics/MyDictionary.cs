namespace Generics;

public class MyDictionary<TKey, TValue>
{
    private List<Entry>?[] _entries;

    public class Entry
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
    public MyDictionary(int capacity = 16)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _entries = new List<Entry>[capacity];
    }


    public void Add(TKey key, TValue value)
    {
        var index = GetIndex(key);

        if (_entries[index] is null)
            _entries[index] = new List<Entry>();

        var bucket = _entries[index]!;

        if (bucket.Any(k => EqualityComparer<TKey>.Default.Equals(k.Key, key)))
            throw new InvalidOperationException("Bunday kalit bilan ma'lumot saqlangan");

        bucket.Add(new Entry(key, value));
    }

    public TValue GetByKey(TKey key)
    {
        var index = GetIndex(key);
        if (_entries[index] is null)
            throw new InvalidOperationException("Bunday kalit bilan ma'lumot topilmadi");

        var entry = _entries[index]!
                .FirstOrDefault(k => EqualityComparer<TKey>.Default.Equals(k.Key, key));

        if (entry is null)
            throw new InvalidOperationException("Bunday kalit bilan ma'lumot topilmadi");

        return entry.Value;
    }

    private int GetIndex(TKey key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        int keyHash = key.GetHashCode() & 0x7fffffff;
        return keyHash % _entries.Length;
    }
}

