using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Repositories;

public class CommentRepository : JsonRepository<Comment>, ICommentRepository
{
    public CommentRepository() : base("Comments") { }

    public async Task DeleteAllByPostId(Guid postId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var allComments = await ReadAll_NoLock();

            int removedCount = allComments.RemoveAll(c => c.PostId == postId);

            if (removedCount > 0)
            {
                await SaveAll_NoLock(allComments);
            }
        }
        finally { _semaphore.Release(); }
    }

    public async Task DeleteRange(List<Guid> ids)
    {
        await _semaphore.WaitAsync();
        try
        {
            var allComments = await ReadAll_NoLock();
            var idsSet = ids.ToHashSet();

            int removedCount = allComments.RemoveAll(c => idsSet.Contains(c.Id));

            if (removedCount > 0)
            {
                await SaveAll_NoLock(allComments);
            }
        }
        finally { _semaphore.Release(); }
    }

    public async Task<IReadOnlyList<Comment>> GetAllByPostId(Guid postId)
    {
        var comments = await GetAll();
        return comments.Where(p => p.PostId == postId)
                       .ToList();
    }

    public async Task<int> GetCountByPostId(Guid postId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var comments = await ReadAll_NoLock();
            return comments.Count(c => c.PostId == postId);
        }
        finally { _semaphore.Release(); }
    }

    public async Task<IReadOnlyList<Comment>> GetUserCommentsInPost(Guid userId, Guid postId)
    {
        var comments = await GetAll();
        return comments.Where(c => c.UserId == userId && c.PostId == postId)
                       .ToList();
    }
}
