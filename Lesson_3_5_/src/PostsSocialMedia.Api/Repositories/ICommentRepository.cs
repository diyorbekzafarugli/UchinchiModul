using AutoMapper.Configuration.Conventions;
using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Repositories;

public interface ICommentRepository
{
    void Add(Comment comment);
    Comment? GetById(Guid id);
    IReadOnlyList<Comment>? GetAll();
    bool Update(Comment commentUpdated);
    bool Delete(Guid id);
    void DeleteRange(List<Guid> ids);
    void DeleteAllByPostId(Guid postId);
    IReadOnlyList<Comment> GetAllByPostId(Guid postId);
    IReadOnlyList<Comment> GetUserCommentsInPost(Guid userId, Guid postId);
    int GetCoutnByPostId(Guid postId);
}
