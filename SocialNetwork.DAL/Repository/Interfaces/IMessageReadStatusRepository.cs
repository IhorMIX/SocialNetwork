using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IMessageReadStatusRepository : IBasicRepository<MessageReadStatus>
{
    public Task UpdateStatus(MessageReadStatus messageReadStatus, CancellationToken cancellationToken = default);
    public Task<IEnumerable<MessageReadStatus>> UpdateStatus(IEnumerable<MessageReadStatus> messageReadStatuses, CancellationToken cancellationToken = default);
}