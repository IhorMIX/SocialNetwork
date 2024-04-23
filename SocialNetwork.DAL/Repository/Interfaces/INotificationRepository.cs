using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface INotificationRepository : IBasicRepository<NotificationEntity>
{
    
    public Task<int> CreateNotification(NotificationEntity notificationEntity,
        CancellationToken cancellationToken = default);    
    public Task CreateNotifications(IEnumerable<NotificationEntity> baseNotificationEntities,
        CancellationToken cancellationToken = default);

    public Task RemoveNotification(IEnumerable<NotificationEntity> baseNotificationEntity, 
        CancellationToken cancellationToken = default);
    public Task RemoveNotification(NotificationEntity notificationEntity, 
        CancellationToken cancellationToken = default);

    public Task UpdateNotification(NotificationEntity notificationEntity,
        CancellationToken cancellationToken = default);
}