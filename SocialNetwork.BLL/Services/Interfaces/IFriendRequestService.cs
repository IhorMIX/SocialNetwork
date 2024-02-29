using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IFriendRequestService : IBaseService<FriendRequestModel>
{
    Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Create friend request and notification about it
    /// </summary>
    /// <returns>notification id</returns>
    Task<int> SendRequest(int userId, int receiverId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create friendship, delete request and create notification about confirmation
    /// </summary>
    /// <returns>notification id</returns>
    Task<int> AcceptRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task CancelRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<FriendRequestModel>> GetAllIncomeRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<FriendRequestModel>> GetAllSentRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
}