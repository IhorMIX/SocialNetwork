using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;


namespace SocialNetwork.DAL.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public GroupRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }
        public IQueryable<Group> GetAll()
        {
            return _socialNetworkDbContext.Groups
            .Include(i => i.GroupMembers)
            .Include(i => i.RoleGroups)
            .AsQueryable();
        }

        public async Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.Groups
                .Include(i => i.GroupMembers)
                .Include(i => i.RoleGroups)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<Group> CreateGroup(Group group, CancellationToken cancellationToken = default)
        {
            var groupEntity = await _socialNetworkDbContext.Groups.AddAsync(group, cancellationToken);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return groupEntity.Entity;
        }

        public async Task DeleteGroupAsync(Group group, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.Groups.Remove(group);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task AddGroupMemberAsync(GroupMember groupMember, Group group,
        CancellationToken cancellationToken = default)
        {
            group.GroupMembers.Add(groupMember);
            _socialNetworkDbContext.Groups.Update(group);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task DelGroupMemberAsync(int groupMemberId, Group group, CancellationToken cancellationToken = default)
        {
            var groupMember = await _socialNetworkDbContext.GroupMembers
                .FirstOrDefaultAsync(i => i.Id == groupMemberId, cancellationToken);

            group.GroupMembers?.Remove(groupMember!);
            _socialNetworkDbContext.Groups.Update(group);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task EditGroup(Group group, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.Groups.Update(group);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsUserInGroupAsync(int userId, Group group, CancellationToken cancellationToken = default)
        {
            var groupMember = await _socialNetworkDbContext.GroupMembers
                .Include(c => c.Group)
                .Include(i => i.RoleGroup)
                .SingleOrDefaultAsync(i => i.User.Id == userId && i.Group.Id == group.Id, cancellationToken);

            return groupMember != null;
        }

    }
}