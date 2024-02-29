using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface INotificationService : IBaseService<BaseNotificationModel>
{
    Task<BaseNotificationModel> CreateNotification(BaseNotificationModel baseNotificationModel,
        CancellationToken cancellationToken = default);

    Task CreateNotifications(IEnumerable<BaseNotificationModel> baseNotificationModel,
        CancellationToken cancellationToken = default);
    Task RemoveNotification(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task ReadNotification(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BaseNotificationModel>> GetByUserId(int userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<BaseNotificationModel>> GetBoxNotificationsByUserId(int userId,
        CancellationToken cancellationToken = default);
}