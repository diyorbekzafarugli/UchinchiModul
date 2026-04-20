namespace StudentCoursePlatform.Domain.Entities;

public class Result<T>
{
    public bool IsSuccess { get;private set; }
    public string[] Errors { get;private set; } = [];
    public T? Data { get;private set; }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<T> Fail(params string[] errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors
        };

    }
}
