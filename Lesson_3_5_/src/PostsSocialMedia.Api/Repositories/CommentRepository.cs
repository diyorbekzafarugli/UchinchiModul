using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Repositories;

public class CommentRepository : JsonRepository<Comment>, ICommentRepository
{
    public CommentRepository() : base("Comments")
    {

    }

    public void DeleteAllByPostId(Guid postId)
    {
        lock (_fileLock)
        {
            var allComments = ReadAll_NoLock();

            int removedCount = allComments.RemoveAll(c => c.PostId == postId);

            if (removedCount > 0)
            {
                SaveAll_NoLock(allComments);
                return;
            }
            return;
        }
    }

    public void DeleteRange(List<Guid> ids)
    {
        lock (_fileLock)
        {
            var allComments = ReadAll_NoLock();
            var idsSet = ids.ToHashSet();

            int removedCount = allComments.RemoveAll(c => idsSet.Contains(c.Id));

            if(removedCount > 0)
            {
                SaveAll_NoLock(allComments);
                return;
            }
            return;
        }
    }

    public IReadOnlyList<Comment> GetAllByPostId(Guid postId)
    {
        return GetAll()
            .Where(p => p.PostId == postId)
            .ToList();
    }

    public int GetCoutnByPostId(Guid postId)
    {
        lock (_fileLock)
        {
            return ReadAll_NoLock().Count(c => c.PostId == postId);
        }
    }

    public IReadOnlyList<Comment> GetUserCommentsInPost(Guid userId, Guid postId)
    {
        return GetAll()
            .Where(c => c.UserId == userId && c.PostId == postId)
            .ToList();
    }
}
