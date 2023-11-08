using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;


namespace SocialNetwork.DAL.Repository
{
    public class BlackListRepository : IBlackListRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public BlackListRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }

        public IQueryable<BlackList> GetAll()
        {
            return _socialNetworkDbContext.BlackLists
                .Include(c => c.BannedUser)
                .Include(c => c.User)
                .AsQueryable();
        }

        public async Task<BlackList?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.BlackLists
                .Include(c => c.BannedUser)
                .Include(c => c.User)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task AddUserAsync(User needToBanUser, BlackList blackList,
            CancellationToken cancellationToken = default)
        {
            blackList.BannedUserId = needToBanUser.Id;
            _socialNetworkDbContext.BlackLists.Add(blackList);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task RemoveUserAsync(BlackList blacklist, CancellationToken cancellationToken = default)
        {
            var blackListEntry = await _socialNetworkDbContext.BlackLists
                .FirstOrDefaultAsync(bl =>
                    (bl.UserId == blacklist.UserId && bl.BannedUserId == blacklist.BannedUserId), cancellationToken);

            if (blackListEntry != null)
            {
                _socialNetworkDbContext.BlackLists.Remove(blackListEntry);
                await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            }
        }


        public IQueryable<BlackList> GetAllBannedUserByUserId(int id)
        {
            var query = _socialNetworkDbContext.BlackLists
                .Include(f => f.User.Profile)
                .Include(f => f.BannedUser.Profile)
                .Where(f => f.UserId == id);

            return query;
        }
    }
}