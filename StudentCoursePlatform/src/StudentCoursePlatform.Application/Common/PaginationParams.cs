namespace StudentCoursePlatform.Application.Common;

public record PaginationParams
{
    public PaginationParams() { }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public PaginationParams(int page, int pageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}