using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Azure.Core;

namespace SocialNetwork.DAL.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public RequestRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }
        public IQueryable<BaseRequestEntity> GetAll()
        {
            return _socialNetworkDbContext.Requests
                .Include(u => u.Sender.Profile)
                .Include(u => (u as FriendRequest)!.ToUser.Profile)
                .Include(u => (u as GroupRequest)!.ToGroup)
                .AsQueryable();
        }

        public async Task<BaseRequestEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.Requests.Include(i => i.Sender).ThenInclude(i => i.Profile).SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<int> CreateRequestAsync(BaseRequestEntity baseRequestEntity, CancellationToken cancellationToken = default)
        {
            var entityEntry = await _socialNetworkDbContext.Requests.AddAsync(baseRequestEntity, cancellationToken);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return entityEntry.Entity.Id;
        }

        public async Task<bool> DeleteRequestAsync(BaseRequestEntity requestEntity, CancellationToken cancellationToken = default)
        {

            if (requestEntity != null && await RequestExists(requestEntity))
            {
                _socialNetworkDbContext.Requests.Remove(requestEntity);
                await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAllRequestAsync(int userId, CancellationToken cancellationToken = default)
        {

            var friendsRequestToRemove = await _socialNetworkDbContext.Requests
                .Where(DeleteRequests(userId))
                .ToListAsync(cancellationToken);

            if (friendsRequestToRemove.Any())
            {
                _socialNetworkDbContext.Requests.RemoveRange(friendsRequestToRemove);
                await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);

                return true;
            }

            return false;
        }

        public async Task<bool> RequestExists(BaseRequestEntity request, CancellationToken cancellationToken = default)
        {
            var result = await _socialNetworkDbContext.Requests.Where(ByRequest(request)).SingleOrDefaultAsync(cancellationToken);

            return result != null;
        }

        /// <summary>
        /// Expression for LINQ RequestExists
        /// </summary>
        public static Expression<Func<BaseRequestEntity, bool>> ByRequest(BaseRequestEntity request)
        {
            switch (request)
            {
                case FriendRequest frr:
                    return fr => (fr is FriendRequest &&
                                 ((FriendRequest)fr).SenderId == frr.SenderId &&
                                 ((FriendRequest)fr).ToUserId == frr.ToUserId) ||
                                (fr is FriendRequest &&
                                 ((FriendRequest)fr).SenderId == frr.ToUserId &&
                                 ((FriendRequest)fr).ToUserId == frr.SenderId);
                case GroupRequest grr:
                    return fr => (fr is GroupRequest &&
                                 ((GroupRequest)fr).SenderId == grr.SenderId &&
                                 ((GroupRequest)fr).ToGroupId == grr.ToGroupId);
                default:
                    throw new Exception("Invalid request type.");
            }
        }

        public static Expression<Func<BaseRequestEntity, bool>> DeleteRequests(int userId)
        {
            return fr => ((fr.SenderId == userId &&
            (fr is FriendRequest && ((FriendRequest)fr).ToUserId == userId))); 
        }


        public IQueryable<BaseRequestEntity> GetAllRequest(int id)
        {
            return _socialNetworkDbContext.Requests
                .Include(f => f.Sender.Profile)
                .Where(f => f.SenderId == id);
        }
    }
}
