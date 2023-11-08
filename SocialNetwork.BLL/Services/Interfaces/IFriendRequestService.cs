using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IFriendRequestService : IBaseService<FriendRequestModel>
{
    Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken);
    Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken);
    Task AcceptRequest(int userId, int requestId, CancellationToken cancellationToken);
    Task CancelRequest(int userId, int requestId, CancellationToken cancellationToken);
    Task<IEnumerable<FriendRequestModel>> GetAllIncomeRequest(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<FriendRequestModel>> GetAllSentRequest(int userId, CancellationToken cancellationToken);
}