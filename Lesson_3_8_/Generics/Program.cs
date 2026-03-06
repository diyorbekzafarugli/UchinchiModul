using System.Text.Json;

namespace Generics;

internal class Program
{
    static void Main(string[] args)
    {
        //List<int> ints = [];

        //MyStack<int> myStack = new();
        //Console.WriteLine(myStack.Capacity);

        //MyQueue<int> ints = new();
        //ints.Enqueue(34);
        //ints.Enqueue(323);
        //ints.Enqueue(78);
        //ints.Enqueue(9);

        //Console.WriteLine(ints.Peek());

        //Wrapper<bool> wrapper = new(true);
        //Console.WriteLine(wrapper);


        //MyDictionary<int, string> myDictionary = new();
        //myDictionary.Add(23, "Azamat");

    }
    static void PrintArray<T>(T[] items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    static T GetFirstElement<T>(List<T> items)
    {
        return items[0];
    }

    static bool AreEqual<T>(T el1, T el2)
    {
        return Equals(el1, el2);
    }


    static T Max<T>(T a, T b) where T : IComparable<T>
    {
        
    }
}
