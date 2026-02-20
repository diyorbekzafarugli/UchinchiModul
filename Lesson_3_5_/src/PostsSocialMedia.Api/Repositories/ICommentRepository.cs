using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Repositories;

public interface ICommentRepository
{
    Task Add(Comment comment);
    Task<Comment?> GetById(Guid id);
    Task<IReadOnlyList<Comment>> GetAll();
    Task Update(Comment commentUpdated);
    Task Delete(Guid id);
    Task DeleteRange(List<Guid> ids);
    Task DeleteAllByPostId(Guid postId);
    Task<IReadOnlyList<Comment>> GetAllByPostId(Guid postId);
    Task<IReadOnlyList<Comment>> GetUserCommentsInPost(Guid userId, Guid postId);
    Task<int> GetCountByPostId(Guid postId);
}
