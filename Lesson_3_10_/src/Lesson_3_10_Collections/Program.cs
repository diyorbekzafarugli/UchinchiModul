namespace Lesson_3_10_Collections;

internal class Program
{
    static void Main(string[] args)
    {

        

        //string str = "Salom bu mening stringdan iborat satrim";
        //string[] arr = str.Split(" ");
        //Stack<string> strings = [];

        //foreach (var item in arr)
        //{
        //    strings.Push(item);
        //}


        //Console.WriteLine(strings.Pop());
        //Console.WriteLine(strings.Pop());
        //Console.WriteLine(strings.Pop());
        //Console.WriteLine(strings.Pop());
        //Console.WriteLine(strings.Pop());
        //Console.WriteLine(strings.Pop());

        //Stack<char> chars = new();
        //string str = "(Salom, {bu mening dasturim}) va u [to'g'ri ishlaydi].)";
        //bool isValid = true;

        //foreach (var ch in str)
        //{
        //    if (ch == '(' || ch == '[' || ch == '{')
        //    {
        //        chars.Push(ch);
        //    }
        //    else if (ch == ')' || ch == ']' || ch == '}')
        //    {
        //        if (chars.Count == 0)
        //        {
        //            isValid = false;
        //            break;
        //        }

        //        var top = chars.Peek();

        //        if (ch == ')' && top == '(' ||
        //            ch == ']' && top == '[' ||
        //            ch == '}' && top == '{')
        //        {
        //            chars.Pop();
        //        }
        //        else
        //        {
        //            isValid = false;
        //            break;
        //        }
        //    }

        //}
        //if (isValid && chars.Count == 0)
        //    Console.WriteLine("To'g'ri");
        //else Console.WriteLine("Noto'g'ri");



        //var books = new List<Book>
        //{
        //    new Book
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Clean Code",
        //        Author = "Robert C. Martin",
        //        Discription = "A handbook of agile software craftsmanship."
        //    },
        //    new Book
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "The Pragmatic Programmer",
        //        Author = "Andrew Hunt & David Thomas",
        //        Discription = "Practical tips and philosophy for modern software developers."
        //    },
        //    new Book
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Design Patterns",
        //        Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
        //        Discription = "Classic book describing reusable object-oriented design patterns."
        //    },
        //    new Book
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Refactoring",
        //        Author = "Martin Fowler",
        //        Discription = "Improving the design of existing code."
        //    },
        //    new Book
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Domain-Driven Design",
        //        Author = "Eric Evans",
        //        Discription = "Tackling complexity in the heart of software."
        //    }
        //};
        //var book1 = books[0];

        //Dictionary<Book, string> keyValuePairs = [];
        //keyValuePairs.Add(book1, book1.Author);
        //keyValuePairs.Add(books[1], books[1].Author);

        //Console.WriteLine(keyValuePairs[book1]);

        //Lookup<int, string> strings;
        //Stack<int> numbers = [];
        //Random random = new();

        //while(numbers.Count < 10)
        //{
        //    numbers.Push(random.Next(1, 100));
        //    var num = numbers.Peek();
        //    if (num % 2 == 0)
        //    {
        //        Console.WriteLine(num);
        //    }
        //}

    }
}
