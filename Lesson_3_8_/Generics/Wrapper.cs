namespace Generics;

public class Wrapper<T> where T : struct 
{
    public T Value { get; set; }
    public Wrapper(T value)
    {
        Value = value;
    }
    public override string ToString()
    {
        return $"Type : {TypeName(typeof(T))}, Value : {Value}";
    }

    private static string TypeName(Type type)
    {
        return type == typeof(int) ? "int"
            : type == typeof(string) ? "string"
            : type == typeof(double) ? "double"
            : type == typeof(char) ? "char"
            : type == typeof(bool) ? "bool"
            : type == typeof(long) ? "long"
            : type == typeof(decimal) ? "decimal"
            : type.Name;
    }
}

