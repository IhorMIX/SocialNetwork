using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IReactionRepository : IBasicRepository<Reaction>
{
    public Task<Reaction> CreateReactionAsync(Reaction reaction, CancellationToken cancellationToken = default);
    public Task EditReactionAsync(Reaction reaction, CancellationToken cancellationToken = default);
    public Task DeleteReactionAsync(Reaction reaction, CancellationToken cancellationToken = default);
}