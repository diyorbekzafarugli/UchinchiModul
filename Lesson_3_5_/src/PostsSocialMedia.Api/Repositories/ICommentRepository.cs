using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface ICommentRepository
{
    void Add(Comment comment);
    Comment? GetById(Guid id);
    IReadOnlyList<Comment>? GetAll();
    bool Update(Comment commentUpdated);
    bool Delete(Guid id);
    IReadOnlyList<Comment> GetAllByPostId(Guid postId);
    IReadOnlyList<Comment> GetAllByUserId(Guid userId, Guid postId);
}
