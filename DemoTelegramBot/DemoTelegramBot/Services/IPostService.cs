using DemoTelegramBot.Dtos;
using DemoTelegramBot.Entities;

namespace DemoTelegramBot.Services;

public interface IPostService
{
    Result<Guid> Add(Token token, string title, string content);
    Result<bool> Update(Token token, Guid postId, string title, string content);
    Result<bool> Delete(Token token, Guid postId);

    Result<PostGetDto> GetById(Token token, Guid postId);
    Result<IReadOnlyList<PostGetDto>> GetMyPosts(Token token);

    Result<IReadOnlyList<PostGetDto>> GetAll(Token token);

    Result<IReadOnlyList<PostGetDto>> GetByUserId(Token token, Guid userId);
}