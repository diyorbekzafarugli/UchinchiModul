using SelfStudy.Dtos;
using SelfStudy.Entities;
using SelfStudy.Repositories;
using SelfStudy.Services;

namespace SelfStudy;

internal class Program
{
    static void Main(string[] args)
    {
        // 1️⃣ Repository va Service yaratish
        IRepository<Product> repository = new ProductRepository();
        IProductService productService = new ProductService(repository);

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=== Product Management ===");
            Console.WriteLine("1. Product qo‘shish");
            Console.WriteLine("2. Productni ID orqali topish");
            Console.WriteLine("3. Barcha productlarni ko‘rish");
            Console.WriteLine("4. Productni o'chirish");
            Console.WriteLine("5. Productni yangilash");
            Console.WriteLine("6. Nomi bo'yicha qidirish");
            Console.WriteLine("7. Narxi bo'yicha qidirish");
            Console.WriteLine("0. Chiqish");
            Console.Write("Tanlovni kiriting: ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1": // Product qo‘shish
                    AddProduct(productService);
                    break;
                case "2": // ID orqali topish
                    FindProductById(productService);
                    break;
                case "3": // Barcha productlarni ko‘rish
                    ShowAllProducts(productService);
                    break;
                case "4": // productni o'chirish
                    DeleteById(productService);
                    break;
                case "5": // productni Yangilash
                    UpdateById(productService);
                    break;
                case "6": // productni nomi bo'yicha qidirish
                    ProductsByName(productService);
                    break;
                case "7": // productni narxi bo'yicha qidirish
                    ProductsExpensive(productService);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Noto‘g‘ri tanlov!");
                    break;
            }
        }
    }

    static void AddProduct(IProductService productService)
    {
        Console.Write("Mahsulot nomini kiriting: ");
        string name = Console.ReadLine()!;

        Console.Write("Mahsulot narxini kiriting: ");
        decimal price = decimal.Parse(Console.ReadLine()!);

        Console.Write("Mahsulot sonini kiriting: ");
        int quantity = int.Parse(Console.ReadLine()!);

        ProductCreateDto dto = new ProductCreateDto
        {
            Name = name,
            Price = price,
            Quantity = quantity
        };

        Guid? id = productService.Create(dto);
        if (id != null)
            Console.WriteLine($"Product qo‘shildi. ID: {id}");
        else
            Console.WriteLine("Product yaratilmadi: noto‘g‘ri qiymat!");
    }

    static void FindProductById(IProductService productService)
    {
        Console.Write("Qidirilayotgan product ID sini kiriting: ");
        Guid id = Guid.Parse(Console.ReadLine()!);

        var product = productService.Get(id);
        if (product != null)
        {
            Console.WriteLine($"Product topildi: {product.Name}, Narxi: {product.Price} UZS, ID: {product.Id}");
        }
        else
        {
            Console.WriteLine("Product topilmadi!");
        }
    }

    static void ShowAllProducts(IProductService productService)
    {
        var products = productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("Hech qanday product yo‘q!");
            return;
        }

        Console.WriteLine("\nBarcha Productlar:");
        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Price: {p.Price}");
        }
    }

    static void DeleteById(IProductService productService)
    {
        Console.Write("Maxsulot ID sin kriting : ");
        var id = Guid.Parse(Console.ReadLine()!);
        var result = productService.Delete(id);
        if (result)
        {
            Console.WriteLine("Muvaffaqiyatli o'chirildi");
        }
        else
        {
            Console.WriteLine("Maxsulot topilamdi");
        }
    }

    static void UpdateById(IProductService productService)
    {
        Console.Write("Maxsulot ID sin kriting : ");
        var id = Guid.Parse(Console.ReadLine()!);

        var result = productService.Get(id);
        if (result != null)
        {
            var updateDto = new ProductUpdateDto()
            {
                Id = result.Id
            };

            Console.Write("Maxsulotning yangi nomini kiriting : ");
            updateDto.Name = Console.ReadLine()!;

            Console.Write("Maxsulotning yangi narxini kiriting : ");
            updateDto.Price = decimal.Parse(Console.ReadLine()!);

            Console.Write("Maxsulotning sonini kiriting : ");
            updateDto.Quantity = int.Parse(Console.ReadLine()!);

            bool updated = productService.Update(updateDto);
            if (updated)
            {
                Console.WriteLine("Maxsulot muvaffaqiaytli yangilandi");
            }
            else
            {
                Console.WriteLine("Yangilanmadi");
            }
        }
        else
        {
            Console.WriteLine("Maxsulot topilamdi");
        }
    }

    static void ProductsByName(IProductService productService)
    {
        Console.Write("Maxsulot nomini kiriting : ");
        string name = Console.ReadLine()!;

        var result = productService.ProductsByName(name);
        if (result.Count == 0)
        {
            Console.WriteLine("Maxsulot topilmadi");
            return;
        }
        foreach (var product in result)
        {
            Console.WriteLine($"ID : {product.Id}\nName : {product.Name}\nNarxi : {product.Price}");
        }
    }

    static void ProductsExpensive(IProductService productService)
    {
        Console.Write("Minimal narxni kiritng : ");
        decimal price = decimal.Parse(Console.ReadLine()!);

        var result = productService.ProductsExspensiveThen(price);
        if (result.Count == 0)
        {
            Console.WriteLine("Maxsulot topilmadi");
            return;
        }
        foreach (var product in result)
        {
            Console.WriteLine($"ID : {product.Id}\nName : {product.Name}\nNarxi : {product.Price}");
        }
    }

}
