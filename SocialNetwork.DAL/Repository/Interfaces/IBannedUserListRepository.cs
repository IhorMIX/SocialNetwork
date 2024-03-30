using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository.Interfaces
{
    public interface IBannedUserListRepository : IBasicRepository<BannedUserList>
    {
        public Task BanGroupMemberAsync(User needToBanUser, Group group, string reason,
            CancellationToken cancellationToken = default);

        public Task UnBanGroupMemberAsync(int needToUnBanUserId, int groupId,
            CancellationToken cancellationToken = default);

        public IQueryable<BannedUserList> GetAllBannedUserByGroupId(int id);
    }
}
