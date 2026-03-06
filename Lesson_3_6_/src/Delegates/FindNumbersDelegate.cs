namespace Delegates;

public class FindNumbersDelegate
{
    public List<int> FindNumbers(List<int> ints, Predicate<int> predicate)
    {
        var result = new List<int>();

        foreach (var num in ints)
        {
            if (predicate(num))
            {
                result.Add(num);
            }
        }

        return result;
    }
}
