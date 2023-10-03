using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IMessageRepository : IBasicRepository<Message>
{
    public Task<Message> CreateMessageAsync(Message message, CancellationToken cancellationToken = default);
    public Task<Message> EditMessageAsync(Message message, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Message message, CancellationToken cancellationToken = default);
}