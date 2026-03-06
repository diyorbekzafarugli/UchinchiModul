namespace Lesson_3_7_Delegate_Anonym;

internal class Program
{
    static void Main(string[] args)
    {
        //var user = new User()
        //{
        //    Name = "John",
        //    Age = 34
        //};
        //var func = (User user) => user.Age > 18 ? "Adult" : "Minor";
        //string result = func(user);

        //var car = new Car
        //{
        //    Model = "Audi",
        //    Year = 2000
        //};

        //var func1 = (Car car) => Console.WriteLine(DateTime.UtcNow.Year - car.Year);
        //func1(car);


        //Func<List<Product>, Product?> selector = products => products.MinBy(product => product.Price);

        //var listProducts = new List<Product>
        //{
        //    new Product { Name = "iPhone 15", Price = 999.99m },
        //    new Product { Name = "Samsung Galaxy S24", Price = 899.50m },
        //    new Product { Name = "Xiaomi Redmi Note 13", Price = 249.99m },
        //    new Product { Name = "MacBook Air M2", Price = 1199.00m },
        //    new Product { Name = "Lenovo ThinkPad E14", Price = 749.99m },
        //    new Product { Name = "AirPods Pro", Price = 249.00m },
        //    new Product { Name = "Sony WH-1000XM5", Price = 329.99m },
        //    new Product { Name = "Apple Watch SE", Price = 279.00m },
        //    new Product { Name = "Logitech MX Master 3S", Price = 99.99m },
        //    new Product { Name = "Dell 27\" Monitor", Price = 219.99m },
        //    new Product { Name = "JBL Flip 6", Price = 129.99m },
        //    new Product { Name = "Kingston 1TB SSD", Price = 79.99m },
        //    new Product { Name = "Razer Keyboard", Price = 149.00m },
        //    new Product { Name = "HP Laser Printer", Price = 189.99m },
        //    new Product { Name = "USB-C Hub", Price = 39.99m },
        //    new Product { Name = "External HDD 2TB", Price = 64.99m },
        //    new Product { Name = "Webcam Full HD", Price = 49.99m },
        //    new Product { Name = "Gaming Mousepad", Price = 14.99m },
        //    new Product { Name = "Office Chair", Price = 159.99m },
        //    new Product { Name = "Portable Charger", Price = 29.99m }
        //};

        //var product = selector(listProducts);
        //Console.WriteLine(product.Price);


        //Action<Order> action = order => Console.WriteLine(order.Price * order.Quantity);
        //var order = new Order
        //{
        //    Name = "IPhone 17 Pro Max",
        //    Price = 1200,
        //    Quantity = 12
        //};

        //action(order);

        //Func<List<Student>, List<Student>> studentsFactory = students => students.Where(s => s.Score > 90).ToList();

        //var students = new List<Student>
        //{
        //    new Student { FirstName = "Ali",      LastName = "Karimov",    Score = 86 },
        //    new Student { FirstName = "Diyor",    LastName = "Rasulov",    Score = 92 },
        //    new Student { FirstName = "Sardor",   LastName = "Ismoilov",   Score = 74 },
        //    new Student { FirstName = "Madina",   LastName = "Qodirova",   Score = 95 },
        //    new Student { FirstName = "Aziza",    LastName = "Tursunova",  Score = 81 },
        //    new Student { FirstName = "Jasur",    LastName = "Nazarov",    Score = 68 },
        //    new Student { FirstName = "Shahzod",  LastName = "Umarov",     Score = 77 },
        //    new Student { FirstName = "Nigora",   LastName = "Abdullayeva",Score = 89 },
        //    new Student { FirstName = "Bekzod",   LastName = "Sattorov",   Score = 90 },
        //    new Student { FirstName = "Malika",   LastName = "Yusupova",   Score = 72 }
        //};

        //var result = studentsFactory(students);

        //Func<Employee, decimal> salaryFactory = e => e.Salary * 1.15m;
        //var employee = new Employee
        //{
        //    FullName = "Sodiqov Abror",
        //    Salary = 5_600_000m
        //};
        //var result = salaryFactory(employee);
        //Console.WriteLine(result);

        //Func<string, int> funcCountVowelsInString = t =>
        //{
        //    if (string.IsNullOrWhiteSpace(t)) return 0;

        //    var vowels = "AEIOUaeiou".ToHashSet();
        //    return t.Count(ch => vowels.Contains(ch));
        //};

        //int result = funcCountVowelsInString("Salom");
        //Console.WriteLine(result);

        //Action<int, int> actionMax = (a, b) => Console.WriteLine(Math.Max(a, b));
        //actionMax(23, 34);

        //Predicate<int> predicate = num => num % 2 == 0 ? true : false;
        //bool isEven = predicate(34);
        //Console.WriteLine(isEven);

        //Func<string, string> stringReversed = s =>
        //{
        //    if (string.IsNullOrWhiteSpace(s)) return string.Empty;

        //    var arr = s.ToCharArray();
        //    Array.Reverse(arr);
        //    return new string(arr);
        //};

        //var str = stringReversed("Salom");
        //Console.WriteLine(str);
    }

}
