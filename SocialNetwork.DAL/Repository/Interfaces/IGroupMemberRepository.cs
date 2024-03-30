using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository.Interfaces
{
    public interface IGroupMemberRepository : IBasicRepository<GroupMember>
    {
        public Task<GroupMember?> GetByUserIdAndGroupId(int userId, int groupId,
       CancellationToken cancellationToken = default);
        public Task UpdateGroupMember(GroupMember groupMember, CancellationToken cancellationToken = default);
        public Task SetRole(List<GroupMember> groupMembers, CancellationToken cancellationToken = default);
    }
}