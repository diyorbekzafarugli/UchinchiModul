namespace StudentCoursePlatform.Application.Interfaces;

public interface ICurrentUserService
{
    public Guid UserId { get; }
    string RawToken { get; }
}
