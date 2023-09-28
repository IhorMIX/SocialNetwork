using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IReactionRepository : IBasicRepository<Reaction>
{
    public Task CreateReaction(Reaction reaction, CancellationToken cancellationToken = default);
    public Task EditReaction(Reaction reaction, CancellationToken cancellationToken = default);
    public Task DeleteReaction(Reaction reaction, CancellationToken cancellationToken = default);
}