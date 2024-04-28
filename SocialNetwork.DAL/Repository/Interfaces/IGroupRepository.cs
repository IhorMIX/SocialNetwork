using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces
{
    public interface IGroupRepository : IBasicRepository<Group>
    {
        Task<Group> CreateGroup(Group group, CancellationToken cancellationToken = default);
        Task DeleteGroupAsync(Group group, CancellationToken cancellationToken = default);
        Task AddGroupMemberAsync(GroupMember groupMember, Group group, CancellationToken cancellationToken = default);
        Task DelGroupMemberAsync(int userId, Group group, CancellationToken cancellationToken = default);
        public Task EditGroup(Group group, CancellationToken cancellationToken = default);
        Task<bool> IsUserInGroupAsync(int userId, Group group, CancellationToken cancellationToken = default);
    }
}