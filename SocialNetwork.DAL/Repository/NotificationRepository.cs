using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class NotificationRepository : INotificationRepository
{

    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public NotificationRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }


    public IQueryable<BaseNotificationEntity> GetAll()
    {
        return _socialNetworkDbContext.Notifications.AsQueryable();
    }

    public async Task<BaseNotificationEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Notifications.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }


    public async Task<BaseNotificationEntity?> GetByIdAsync(int id, NotificationType notificationType, CancellationToken cancellationToken = default)
    {
        return notificationType switch
        {
            NotificationType.FriendRequest => await _socialNetworkDbContext.Notifications.OfType<FriendRequestNotification>()
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken),
            
            _ => throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null)
        };
    }

    public async Task<int> CreateNotification(BaseNotificationEntity baseNotificationEntity,
        CancellationToken cancellationToken = default)
    {
        var entityEntry = await _socialNetworkDbContext.Notifications.AddAsync(baseNotificationEntity, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity.Id;
    }

    public async Task RemoveNotification(BaseNotificationEntity baseNotificationEntity,
        CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Notifications.Remove(baseNotificationEntity);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateNotification(BaseNotificationEntity baseNotificationEntity,
        CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Notifications.Update(baseNotificationEntity);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}