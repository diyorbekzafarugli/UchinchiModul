namespace Generics;

public class Pair<T>
{
    private List<T> Items { get; set; }
    private T Value { get; set; }
    public Pair(List<T> items, T value)
    {
        Items = items;
        Value = value;
    }
}
