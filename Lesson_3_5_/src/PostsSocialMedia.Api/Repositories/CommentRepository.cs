using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Repositories;

public class CommentRepository : JsonRepository<Comment>, ICommentRepository
{
    public CommentRepository() : base("Comments")
    {

    }

    public IReadOnlyList<Comment> GetAllByPostId(Guid postId)
    {
        return GetAll()
            .Where(p => p.PostId == postId)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<Comment> GetAllByUserId(Guid userId, Guid postId)
    {
        return GetAll()
            .Where(c => c.UserId == userId && c.PostId == postId)
            .ToList()
            .AsReadOnly();
    }
}
