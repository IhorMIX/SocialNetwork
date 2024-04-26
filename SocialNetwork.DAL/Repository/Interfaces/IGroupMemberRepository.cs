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
        public Task<GroupMember?> GetByUserIdAndGroupIdAsync(int userId, int groupId,
       CancellationToken cancellationToken = default);
        public Task UpdateGroupMemberAsync(GroupMember groupMember, CancellationToken cancellationToken = default);
        public Task UpdateGroupMemberAsync(List<GroupMember> groupMembers, CancellationToken cancellationToken = default);
    }
}