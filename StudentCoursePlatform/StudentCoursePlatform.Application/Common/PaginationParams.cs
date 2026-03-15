namespace StudentCoursePlatform.Application.Common;

public record PaginationParams
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public PaginationParams(int page, int pageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}
