using SocialNetwork.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services.Interfaces
{
    public interface IGroupService : IBaseService<GroupModel>
    {
        public Task<GroupModel> CreateGroup(int userId, GroupModel groupModel, CancellationToken cancellationToken = default);
        public Task DeleteGroup(int userId, int groupId, CancellationToken cancellationToken = default);
        public Task<GroupModel> EditGroup(int userId, int groupId, GroupModel groupModel, CancellationToken cancellationToken = default);
        public Task JoinGroup(int groupId, int userId, CancellationToken cancellationToken = default);
        public Task LeaveGroup(int groupId, int userId, CancellationToken cancellationToken = default);
        public Task KickMember(int userId, int groupId, int kickedMember, CancellationToken cancellationToken = default);
        public Task MakeHost(int userId, int groupId, int user2Id, CancellationToken cancellationToken = default);
        public Task AddRole(int userId, int groupId, RoleGroupModel roleGroupModel, CancellationToken cancellationToken = default);
        public Task DelRole(int userId, int groupId, int roleId, CancellationToken cancellationToken = default);
        public Task<RoleGroupModel> EditRole(int userId, int groupId, int roleId, RoleGroupModel roleGroupModel,
        CancellationToken cancellationToken = default);
        public Task SetRole(int userId, int groupId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default);
        public Task UnSetRole(int userId, int groupId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default);
        public Task BanGroupMember(int userId, int groupId, int memberToBanId, string reason, CancellationToken cancellationToken = default);
        public Task UnBanGroupMember(int userId, int groupId, int memberToUnBanId, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<BannedUserInGroupModel>> GetAllBannedUser(int userId,int groupId, PaginationModel pagination, CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<RoleGroupModel>> GetAllGroupRoles(int userId, PaginationModel pagination, int groupId,
        CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<GroupModel>> FindGroupByName(int userId, PaginationModel pagination, string groupName,
            CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<GroupModel>> GetAllGroups(int userId, PaginationModel pagination,
            CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<GroupMemberModel>> GetGroupMembers(int userId, PaginationModel pagination, int groupId, int roleGroupId,
            CancellationToken cancellationToken = default);
        public Task<PaginationResultModel<GroupMemberModel>> GetGroupMembers(int userId, PaginationModel pagination, int groupId,
            CancellationToken cancellationToken = default);
    }
}