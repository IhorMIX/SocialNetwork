using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface INotificationRepository : IBasicRepository<BaseNotificationEntity>
{

    public Task<BaseNotificationEntity?> GetByIdAsync(int id, NotificationType notificationType,
        CancellationToken cancellationToken = default);

    public Task<int> CreateNotification(BaseNotificationEntity baseNotificationEntity,
        CancellationToken cancellationToken = default);    
    public Task CreateNotifications(List<BaseNotificationEntity> baseNotificationEntities,
        CancellationToken cancellationToken = default);

    public Task RemoveNotification(BaseNotificationEntity baseNotificationEntity, 
        CancellationToken cancellationToken = default);

    public Task UpdateNotification(BaseNotificationEntity baseNotificationEntity,
        CancellationToken cancellationToken = default);
}