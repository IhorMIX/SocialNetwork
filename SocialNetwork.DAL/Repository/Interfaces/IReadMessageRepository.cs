using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IReadMessageRepository : IBasicRepository<ReadMessage>
{
    public Task AddReadAsync(ReadMessage readMessage, CancellationToken cancellationToken = default);
}