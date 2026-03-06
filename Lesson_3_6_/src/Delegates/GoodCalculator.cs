namespace Delegates;


public delegate int MyDelegate(int num, int num2);
public class GoodCalculator
{
    public int Add(int a, int b) => a + b;

    public int Subtract(int a, int b) => a > b ? a - b : b - a;

    public void PerfomCalculation(int x, int y, Func<int, int, int> myDelegate)
    {
        var result = myDelegate(x, y);
        Console.WriteLine(result);
    }
}
