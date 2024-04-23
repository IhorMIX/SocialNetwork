using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services.Interfaces
{
    public interface IRequestService : IBaseService<BaseRequestModel>
    {
        public Task<int> SendFriendRequestAsync(FriendRequestModel friendRequestModel, CancellationToken cancellationToken = default);
        public Task AcceptFriendRequest(int userId, int requestId, CancellationToken cancellationToken = default);
        public Task CancelFriendRequest(int userId, int requestId, CancellationToken cancellationToken = default);
        public Task<int> SendGroupRequestAsync(GroupRequestModel groupRequestModel, CancellationToken cancellationToken = default);
        public Task AcceptGroupRequest(int userId, int requestId, CancellationToken cancellationToken = default);
        public Task CancelGroupRequest(int userId, int requestId, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<BaseRequestModel>> GetAllSentFriendRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<BaseRequestModel>> GetAllSentGroupRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<BaseRequestModel>> GetAllIncomeFriendRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<BaseRequestModel>> GetAllIncomeGroupRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
    }
}