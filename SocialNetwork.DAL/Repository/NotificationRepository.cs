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


    public IQueryable<NotificationEntity> GetAll()
    {
        return _socialNetworkDbContext.Notifications.AsQueryable();
    }

    public async Task<NotificationEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Notifications.Include(i => i.Initiator).ThenInclude(i => i.Profile).SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
    
    public async Task<int> CreateNotification(NotificationEntity notificationEntity,
        CancellationToken cancellationToken = default)
    {
        var entityEntry = await _socialNetworkDbContext.Notifications.AddAsync(notificationEntity, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity.Id;
    }

    public async Task CreateNotifications(IEnumerable<NotificationEntity> baseNotificationEntities, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Notifications.AddRangeAsync(baseNotificationEntities, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveNotification(IEnumerable<NotificationEntity> baseNotificationEntity,
        CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Notifications.RemoveRange(baseNotificationEntity);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveNotification(NotificationEntity notificationEntity,
        CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Notifications.Remove(notificationEntity);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateNotification(NotificationEntity notificationEntity,
        CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Notifications.Update(notificationEntity);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}