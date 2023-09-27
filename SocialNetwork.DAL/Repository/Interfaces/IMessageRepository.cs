using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IMessageRepository : IBasicRepository<Message>
{
    public Task CreateMessage(Message message, CancellationToken cancellationToken = default);
    public Task EditMessage(Message message, CancellationToken cancellationToken = default);
}