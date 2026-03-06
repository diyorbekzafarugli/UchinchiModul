namespace Generics;

public class Box<T>
{
	private T? Value;

	public T MyProperty
	{
		get { return Value; }
		set { Value = value; }
	}


	public T GetValue(T item)
	{
		return item;
	}
}