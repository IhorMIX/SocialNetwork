using SocialNetwork.BLL.Models;
namespace SocialNetwork.BLL.Services.Interfaces;

public interface INotificationCreationService<TModel, TReturn>
{
    public Task<TReturn> CreateNotification(TModel model, IEnumerable<int> connectedUsersIds, CancellationToken cancellationToken = default);
}