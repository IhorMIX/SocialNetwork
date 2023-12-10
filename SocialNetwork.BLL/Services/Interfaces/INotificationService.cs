using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface INotificationService : IBaseService<NotificationModel>
{
    
    Task<NotificationModel?> GetByIdAsync(int id, NotificationType notificationType, CancellationToken cancellationToken = default);
    Task<NotificationModel> CreateNotification(NotificationModel notificationModel,
        CancellationToken cancellationToken = default);
    Task RemoveNotification(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task ReadNotification(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationModel>> GetByUserId(int userId, CancellationToken cancellationToken = default);
}