using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Repositories;

public class ReactionRepository : JsonRepository<Reaction>, IReactionRepository
{
    public ReactionRepository() : base("Reactions")
    {

    }

    public int DeleteByTargetId(Guid targetId)
    {
        lock (_fileLock)
        {
            var allReactions = ReadAll_NoLock();
            var count = allReactions.RemoveAll(r => r.TargetId == targetId);
            SaveAll_NoLock(allReactions);
            return count;
        }
    }

    public void DeleteByTargetIds(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return;

        lock (_fileLock)
        {
            var allReactions = ReadAll_NoLock();
            var targetSet = ids.ToHashSet();

            int removedCount = allReactions.RemoveAll(r => targetSet.Contains(r.TargetId));

            if(removedCount > 0)
            {
                SaveAll_NoLock(allReactions);
            }
        }
    }

    public List<Reaction> GetByTargetId(Guid targetId)
    {
        lock (_fileLock)
        {
            return ReadAll_NoLock().Where(r => r.TargetId == targetId).ToList() ?? new List<Reaction>();
        }
    }

    public Reaction? GetByUserAndTarget(Guid userId, Guid targetId)
    {
        lock (_fileLock)
        {
            var allReactions = ReadAll_NoLock();
            return allReactions.FirstOrDefault(r => r.UserId == userId && r.TargetId == targetId);
        }
    }
}
