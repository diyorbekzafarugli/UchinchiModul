using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Repositories;

public class ReactionRepository : JsonRepository<Reaction>, IReactionRepository
{
    public ReactionRepository() : base("Reactions")
    {
        
    }
}
