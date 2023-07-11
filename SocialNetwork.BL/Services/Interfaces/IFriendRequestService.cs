using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IFriendRequestService : IBaseService<FriendRequestModel>
{
    Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken);
    Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken);
    Task AcceptRequest(int userId, int senderId, CancellationToken cancellationToken);
    Task CancelRequest(int userId, int senderId, CancellationToken cancellationToken);
    Task<IEnumerable<FriendRequestModel>> GetAllRequest(int userId, CancellationToken cancellationToken);
}