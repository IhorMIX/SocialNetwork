using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class MessageReadStatusRepository : IMessageReadStatusRepository
{
    
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public MessageReadStatusRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }
    
    public IQueryable<MessageReadStatus> GetAll()
    {
        return _socialNetworkDbContext.MessageReadStatuses
            .Include(r => r.ChatMember)
            .Include(r => r.Message)
            .AsQueryable();
    }

    public async Task<MessageReadStatus?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.MessageReadStatuses
            .Include(r => r.ChatMember)
            .Include(r => r.Message)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task UpdateStatus(MessageReadStatus messageReadStatus, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.MessageReadStatuses.Update(messageReadStatus);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatus(IEnumerable<MessageReadStatus> messageReadStatuses, CancellationToken cancellationToken = default)
    {
        
        foreach (var messageReadStatus in messageReadStatuses)
        {
            // Load the existing entity from the database
            var existingMessageReadStatus = await _socialNetworkDbContext.MessageReadStatuses
                .FirstOrDefaultAsync(mrs => mrs.ChatMemberId == messageReadStatus.ChatMemberId && mrs.MessageId == messageReadStatus.MessageId, cancellationToken);

            if (existingMessageReadStatus != null)
            {
                // Update only the IsRead property
                existingMessageReadStatus.IsRead = messageReadStatus.IsRead;
                // You can also update other properties if needed
                // Mark the entity as modified
                _socialNetworkDbContext.Entry(existingMessageReadStatus).State = EntityState.Modified;
            }
        }

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);

        
    }
}