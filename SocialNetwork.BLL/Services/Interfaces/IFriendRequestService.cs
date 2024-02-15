using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IFriendRequestService : IBaseService<FriendRequestModel>
{
    Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken = default);
    Task<int> SendRequest(int userId, int receiverId, CancellationToken cancellationToken = default);
    Task<int> AcceptRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task CancelRequest(int userId, int requestId, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<FriendRequestModel>> GetAllIncomeRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<FriendRequestModel>> GetAllSentRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
}