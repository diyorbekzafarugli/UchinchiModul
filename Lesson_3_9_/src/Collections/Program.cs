using System.Collections;

namespace Collections;

internal class Program : MyClass
{
    static void Main(string[] args)
    {
        List<int> numbers = [];
        Random random = new();
        HashSet<int> ints = [];

        while(ints.Count < 10)
        {
            ints.Add(random.Next(1, 100));
        }

        numbers = [.. ints];

        //for (int i = 0; i < numbers.Count - 1; i++)
        //{

        //    for (int j = i + 1; j < numbers.Count; j++)
        //    {
        //        if (numbers[i] < numbers[j])
        //        {
        //            var temp = numbers[i];
        //            numbers[i] = numbers[j];
        //            numbers[j] = temp;
        //        }
        //    }
        //}
        //var count = numbers.Count;

        //for (int i = 0; i < count - 1; i++)
        //{
        //    for (int j = 0; j < count - i - 1; j++)
        //    {
        //        if (numbers[j] < numbers[j + 1])
        //        {
        //            var temp = numbers[j];
        //            numbers[j] = numbers[j + 1];
        //            numbers[j + 1] = temp;
        //        }
        //    }
        //}

        //foreach (var num in numbers)
        //{
        //    Console.WriteLine(num);
        //}
        //ints.AddLast(45);
        //ints.AddFirst(-90);
        //var node = ints.Find(-90)!;
        //ints.AddBefore(node, 98);
        //ints.AddAfter(node, 209);
        //foreach (var item in ints)
        //{
        //    Console.WriteLine(item);
        //}
        //ArrayList arrayList = [];
        //arrayList.Add("Salom");
        //arrayList.Add(45);
        //arrayList.Add(true);
        //arrayList.Add(new List<int>() { 34, 343, 23, 232, 2, 3, 2, 2, 2, 1 });
        //arrayList.Add(89);
        //arrayList.Add('c');
        //List<int> ints = [];

        ////var numbers = arrayList.OfType<int>().ToList();

        //foreach (var item in arrayList)
        //{
        //    if (item is List<int> list)
        //    {
        //        foreach (var item1 in list)
        //        {
        //            Console.WriteLine(item1);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine(item);
        //    }
        //}



        //var sortedStringArray = arrayList.OfType<string>();
        //sortedStringArray.OrderBy(s => s.Length);


        //foreach (var str in sortedStringArray)
        //{
        //    Console.WriteLine(str);
        //}

        //arrayList.Clear();
        //Console.WriteLine(arrayList.Count);


        //Random random = new();
        //while (arrayList.Count < 10)
        //{
        //    arrayList.Add(random.Next(1, 11));
        //}

        //Console.WriteLine(JsonSerializer.Serialize(arrayList));
    }

    public override int SummNumbers(int a, int b)
    {
        return (a - b) * (a + b) * (a / b);
    }
    public override string GetString(Guid id)
    {
        throw new NotImplementedException();
    }
}

