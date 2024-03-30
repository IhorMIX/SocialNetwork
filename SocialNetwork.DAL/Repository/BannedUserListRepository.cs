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
    public class BannedUserListRepository : IBannedUserListRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public BannedUserListRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }
        public IQueryable<BannedUserList> GetAll()
        {
            return _socialNetworkDbContext.BannedUserLists
               .Include(c => c.Group)
               .Include(c => c.User)
               .AsQueryable();
        }

        public async Task<BannedUserList?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.BannedUserLists
                .Include(c => c.Group)
                .Include(c => c.User)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task BanGroupMemberAsync(User needToBanUser, Group group, string reason,
            CancellationToken cancellationToken = default)
        {
            var bannedUser = new BannedUserList
            {
                Group = group,
                User = needToBanUser,
                Reason = reason
            };
            _socialNetworkDbContext.BannedUserLists.Add(bannedUser!);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UnBanGroupMemberAsync(int needToUnBanUserId, int groupId, 
            CancellationToken cancellationToken = default)
        {
            var bannedUser = await _socialNetworkDbContext.BannedUserLists
                 .Where(b => b.User.Id == needToUnBanUserId /*&& b.Group.Id == groupId*/)
                 .SingleOrDefaultAsync(cancellationToken);
            _socialNetworkDbContext.BannedUserLists.Remove(bannedUser!);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }
        public IQueryable<BannedUserList> GetAllBannedUserByGroupId(int id)
        {
            var query = _socialNetworkDbContext.BannedUserLists
                .Include(f => f.User.Profile)
                .Include(f => f.Group)
                .Where(f => f.GroupId == id);

            return query;
        }
    }
}
