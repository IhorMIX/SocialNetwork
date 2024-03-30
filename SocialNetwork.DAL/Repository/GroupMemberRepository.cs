using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public GroupMemberRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }

        public IQueryable<GroupMember> GetAll()
        {
            return _socialNetworkDbContext.GroupMembers
            .Include(c => c.Group)
            .Include(c => c.RoleGroup)
            .Include(c => c.User)
            .ThenInclude(u => u.Profile)
            .AsQueryable();
        }
        public async Task<GroupMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.GroupMembers
           .Include(c => c.Group)
           .Include(c => c.RoleGroup)
           .Include(c => c.User)
           .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }
        public async Task<GroupMember?> GetByUserIdAndGroupId(int userId, int groupId,
        CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.GroupMembers
                .Include(c => c.Group)
                .Include(c => c.RoleGroup)
                .ThenInclude(r => r.RoleAccesses)
                .Include(c => c.User).ThenInclude(i => i.Profile)
                .FirstOrDefaultAsync(i => i.User.Id == userId && i.Group.Id == groupId, cancellationToken);
        }
        public async Task UpdateGroupMember(GroupMember groupMember, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.GroupMembers.Update(groupMember);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task SetRole(List<GroupMember> groupMembers, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.GroupMembers.UpdateRange(groupMembers);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}