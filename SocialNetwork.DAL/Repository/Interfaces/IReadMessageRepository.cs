using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IReadMessageRepository : IBasicRepository<ReadMessage>
{
    public Task CreateReactionAsync(ReadMessage readMessage, CancellationToken cancellationToken = default);
}