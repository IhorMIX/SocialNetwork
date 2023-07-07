using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IFriendReqestService : IBaseService<FriendRequestModel>
{
    Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken);
    Task AcceptRequest(int userId, int friendRequestId, CancellationToken cancellationToken);
    Task CancelRequest(int userId, int friendRequestId, CancellationToken cancellationToken);
    Task<IEnumerable<FriendRequestModel>> GetAllRequest(int userId, CancellationToken cancellationToken);
}