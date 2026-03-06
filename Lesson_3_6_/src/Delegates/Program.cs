using System.Text.Json;

namespace Delegates;


internal class Program
{
    //static void BarReporter(int percent)
    //{
    //    Console.CursorLeft = 0;
    //    var bars = percent / 10;
    //    var progressBar = "[" + new string('=', bars) + new string(' ', 10 - bars) + "]";
    //    Console.Write($"{progressBar} {percent}% complete");
    //}
    static void Main(string[] args)
    {

        Func<int, int, int, int> func = (int a, int b, int c) => Math.Max(a, Math.Max(b, c));

        Action<string, string> action = (string text1, string text2) => Console.WriteLine(text1.Length + text2.Length);

        Func<List<Book>, Book> function = (List<Book> books) =>
        {
            var bookResult = books[0];
            foreach (var book in books)
            {
                if (book.Price > bookResult.Price)
                {
                    bookResult = book;
                }
            }
            return bookResult;
        };


        Action<Book> action1 = (Book book) => Console.WriteLine(book.Price * 10);

        //var numbers = new List<int>
        //{
        //    3, 7, 12, 19, 25, 31, 44, 50, 63, 78,
        //    81, 92, 105, 118, 120, 134, 147, 159, 172, 200
        //};

        //var result = numbers.Where(func).ToList();


        var books = new List<Book>
        {
            new Book
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Clean Code",
                Author = "Robert C. Martin",
                Discription = "Kod yozishdagi tozalik, naming va refactoring asoslari.",
                Price = 39.99m
            },
            new Book
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "The Pragmatic Programmer",
                Author = "Andrew Hunt, David Thomas",
                Discription = "Professional dasturchi odatlari va amaliy yondashuvlar.",
                Price = 42.50m
            },
            new Book
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Design Patterns",
                Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                Discription = "GoF design patternlar: Factory, Singleton, Adapter va boshqalar.",
                Price = 55.00m
            },
            new Book
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Refactoring",
                Author = "Martin Fowler",
                Discription = "Kod sifati va strukturani yaxshilash uchun refactoring usullari.",
                Price = 49.90m
            },
            new Book
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "C# in Depth",
                Author = "Jon Skeet",
                Discription = "C# tilining chuqur mavzulari: generics, LINQ, async va boshqalar.",
                Price = 46.75m
            },
            new Book
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "ASP.NET Core in Action",
                Author = "Andrew Lock",
                Discription = "ASP.NET Core arxitekturasi, middleware, DI va real loyihalar.",
                Price = 44.00m
            },
            new Book
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "Head First Design Patterns",
                Author = "Eric Freeman, Elisabeth Robson",
                Discription = "Patternlarni oson va ko‘rgazmali uslubda tushuntiradi.",
                Price = 37.99m
            },
            new Book
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Name = "Introduction to Algorithms",
                Author = "Thomas H. Cormen",
                Discription = "Algoritmlar va data structurelar: sorting, graph, DP va boshqalar.",
                Price = 69.00m
            },
            new Book
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Name = "Effective C#",
                Author = "Bill Wagner",
                Discription = "C# da eng yaxshi amaliyotlar: performance, clean code, pitfalls.",
                Price = 33.25m
            },
            new Book
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Domain-Driven Design",
                Author = "Eric Evans",
                Discription = "DDD: domain model, bounded context, aggregate, repository.",
                Price = 58.80m
            },
            new Book
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Building Microservices",
                Author = "Sam Newman",
                Discription = "Microservice arxitekturasi: service boundaries, deployment, monitoring.",
                Price = 52.10m
            },
            new Book
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Kubernetes Up & Running",
                Author = "Kelsey Hightower, Brendan Burns, Joe Beda",
                Discription = "Kubernetes asoslari: pods, services, deployments, scaling.",
                Price = 47.00m
            },
            new Book
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "SQL Performance Explained",
                Author = "Markus Winand",
                Discription = "SQL query optimizatsiyasi va indekslar qanday ishlashi.",
                Price = 29.99m
            },
            new Book
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Name = "You Don't Know JS Yet",
                Author = "Kyle Simpson",
                Discription = "JavaScript engine, scope, closures va async tushunchalari.",
                Price = 24.50m
            },
            new Book
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Name = "The Mythical Man-Month",
                Author = "Frederick P. Brooks Jr.",
                Discription = "Software loyihalarni boshqarishdagi xatolar va saboqlar.",
                Price = 21.99m
            }
        };
        Console.WriteLine(Environment.ProcessorCount); // logical core soni

        //Book book = function(books);
        //Console.WriteLine(JsonSerializer.Serialize(book, new JsonSerializerOptions{ WriteIndented = true }));
        //var dataProcessor = new DataProcessor(BarReporter);
        //await dataProcessor.ProcessData(100);

        //var findNumbers = new FindNumbersDelegate();

        //var ints = findNumbers.FindNumbers(numbers, num => num % 3 == 0 && num % 5 == 0);


        //Console.WriteLine($"{string.Join(", ", ints)}");




        //var users = new List<User>
        //{
        //    new User { Id = Guid.NewGuid(), Name = "Ali Karimov",     Email = "ali.karimov@example.com",     Age = 22, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Diyorbek Azizov", Email = "diyorbek.azizov@example.com", Age = 19, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Sardor Rustamov", Email = "sardor.rustamov@example.com", Age = 28, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Nodira Yusupova", Email = "nodira.yusupova@example.com", Age = 25, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Malika Rahimova", Email = "malika.rahimova@example.com", Age = 31, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Javohir Islomov", Email = "javohir.islomov@example.com", Age = 24, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Aziza Qodirova",  Email = "aziza.qodirova@example.com",  Age = 20, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Bekzod Tursunov", Email = "bekzod.tursunov@example.com", Age = 27, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Shahnoza Aliyeva",Email = "shahnoza.aliyeva@example.com",Age = 23, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Umidjon Ergashev",Email = "umidjon.ergashev@example.com",Age = 29, IsActive = true  },

        //    new User { Id = Guid.NewGuid(), Name = "Farruh Saidov",  Email = "farruh.saidov@example.com",   Age = 26, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Madina Toirova", Email = "madina.toirova@example.com",  Age = 18, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Kamron Soliyev", Email = "kamron.soliyev@example.com",  Age = 33, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Gulnoza Akbarova",Email = "gulnoza.akbarova@example.com",Age = 21, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Sherzod Nazarov",Email = "sherzod.nazarov@example.com", Age = 30, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Ziyoda Mamatova",Email = "ziyoda.mamatova@example.com", Age = 24, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Otabek Nurmatov",Email = "otabek.nurmatov@example.com", Age = 35, IsActive = true  },
        //    new User { Id = Guid.NewGuid(), Name = "Nilufar Raxmonova",Email = "nilufar.raxmonova@example.com",Age = 27, IsActive = true },
        //    new User { Id = Guid.NewGuid(), Name = "Sanjar Eshonqulov",Email = "sanjar.eshonqulov@example.com",Age = 22, IsActive = false },
        //    new User { Id = Guid.NewGuid(), Name = "Laylo Abdullayeva",Email = "laylo.abdullayeva@example.com",Age = 29, IsActive = true  }
        //};

        //var processor = new GoodUserProcessor();
        //processor.ProcessUser(users, user => Console.WriteLine($"ID : {user.Id}\tName : {user.Name}\tEmail : {user.Email}\tAge : {user.Age}"));
        //processor.ProcessUser(users, user => Console.WriteLine($"Simulating sending welcome email to {user.Email}..."));
        //processor.ProcessUser(users, user => user.IsActive = false);
        //processor.ProcessUser(users, user => Console.WriteLine($"Name : {user.Name}\tIsActive : {user.IsActive}"));

        //int x = 45;
        //int y = 5;

        //var operation = new MyDelegate(SummNumbers);


        //GoodCalculator goodCalculator = new();

        //goodCalculator.PerfomCalculation(x, y, (a, b) => a + b);
        //goodCalculator.PerfomCalculation(x, y, (a, b) => a - b);
        //goodCalculator.PerfomCalculation(x, y, (a, b) => a * b);
        //goodCalculator.PerfomCalculation(x, y, (a, b) => a / b);
        //Console.WriteLine("Hello, World!");

        //Action<string, int> action;
        //action = SendMessage;
        //action += Foo;
        //action += Print;
        //action += AddText;

        //action.Invoke("Salom", 23);

        //Predicate<int> predicate;
        //predicate = IsValid;

        //var result = predicate.Invoke(34);

        //Func<string, string, string> factory;
        //factory = AddTexts;
    }

    static int SummNumbers(int a, int b) => (a + b) % b;
    static bool IsValid(int num)
    {
        return num > 0;
    }
    static string AddTexts(string text1, string text2)
    {
        return text1 + text2;
    }
    static void SendMessage(string message, int num)
    {
        Console.WriteLine(message + num);
    }
    static void Foo(string text, int number)
    {
        text += number;
    }
    static void AddText(string text, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                text += text;
            }
        }
    }
    static void Print(string text, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(text);
            }
        }
    }
}