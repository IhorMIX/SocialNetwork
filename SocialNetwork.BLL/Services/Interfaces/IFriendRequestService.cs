using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IFriendRequestService : IBaseService<FriendRequestModel>
{
    Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken = default);
    Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken = default);
    Task AcceptRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task CancelRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FriendRequestModel>> GetAllIncomeRequest(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FriendRequestModel>> GetAllSentRequest(int userId, CancellationToken cancellationToken = default);
}