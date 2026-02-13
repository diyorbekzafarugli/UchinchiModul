using DemoTelegramBot.Entities;

namespace DemoTelegramBot.Repositories;

public interface IPostRepository
{
    Guid Add(Post post);

    bool Update(Guid userId, Guid postId, string title, string content, DateTime updatedAt);
    bool Delete(Guid userId, Guid postId);

    Post? GetById(Guid userId, Guid postId);

    IReadOnlyList<Post> GetAll();
    List<Post> GetByUserId(Guid userId);
}
