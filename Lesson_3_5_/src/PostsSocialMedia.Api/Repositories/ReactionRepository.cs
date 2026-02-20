using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Repositories;

public class ReactionRepository : JsonRepository<Reaction>, IReactionRepository
{
    public ReactionRepository() : base("Reactions")
    {

    }

    public async Task<int> DeleteByTargetId(Guid targetId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var allReactions = await ReadAll_NoLock();
            var count = allReactions.RemoveAll(r => r.TargetId == targetId);
            if (count > 0)
            {
                await SaveAll_NoLock(allReactions);
            }
            return count;
        }
        finally { _semaphore.Release(); }
    }

    public async Task DeleteByTargetIds(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return;

        await _semaphore.WaitAsync();
        try
        {
            var allReactions = await ReadAll_NoLock();
            var targetSet = ids.ToHashSet();

            int removedCount = allReactions.RemoveAll(r => targetSet.Contains(r.TargetId));

            if (removedCount > 0)
            {
                await SaveAll_NoLock(allReactions);
            }
        }
        finally { _semaphore.Release(); }
    }

    public async Task<List<Reaction>> GetByTargetId(Guid targetId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var allReaction = await ReadAll_NoLock();
            return allReaction.Where(r => r.TargetId == targetId).ToList() ?? new List<Reaction>();
        }
        finally { _semaphore.Release(); }
    }

    public async Task<Reaction?> GetByUserAndTarget(Guid userId, Guid targetId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var allReactions = await ReadAll_NoLock();
            return allReactions.FirstOrDefault(r => r.UserId == userId && r.TargetId == targetId);
        }
        finally { _semaphore.Release(); }
    }
}
