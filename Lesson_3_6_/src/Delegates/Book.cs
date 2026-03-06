namespace Delegates;

public class Book
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Discription { get; set; }
    public decimal Price { get; set; }

    //public override string ToString()
    //{
    //    return $"Id: {Id}\nName: {Name}\nAuthor: {Author}\nDesc: {Discription}\nPrice: {Price:C}";
    //}
}