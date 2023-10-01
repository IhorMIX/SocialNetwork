using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IMessageRepository : IBasicRepository<Message>
{
    public Task CreateMessageAsync(Message message, CancellationToken cancellationToken = default);
    public Task EditMessageAsync(Message message, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Message message, CancellationToken cancellationToken = default);
}