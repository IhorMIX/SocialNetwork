using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Services;

namespace SocialNetwork.DAL.Repository;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly CacheService<FriendRequest?> _cacheService;

    public FriendRequestRepository(SocialNetworkDbContext socialNetworkDbContext,
        CacheService<FriendRequest?> cacheService)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _cacheService = cacheService;
    }

    public IQueryable<FriendRequest> GetAll()
    {
        return _socialNetworkDbContext.FriendRequests
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .AsQueryable();
    }

    public async Task<FriendRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrSetAsync($"FriendRequest-{id}", async (token) => 
            await _socialNetworkDbContext
            .FriendRequests
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .FirstOrDefaultAsync(i => i.Id == id, token), cancellationToken, _socialNetworkDbContext);
    }

    public async Task<bool> DeleteFriendRequestAsync(FriendRequest friendRequest,
        CancellationToken cancellationToken = default)
    {
        var friendRequestToRemove = await _socialNetworkDbContext.FriendRequests
            .Where(f =>
                (f.SenderId == friendRequest.SenderId && f.ReceiverId == friendRequest.ReceiverId))
            .SingleOrDefaultAsync(cancellationToken);

        if (friendRequestToRemove != null)
        {
            _socialNetworkDbContext.FriendRequests.Remove(friendRequestToRemove);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveFromCacheAsync($"FriendRequest-{friendRequestToRemove.Id}", cancellationToken);
            
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteAllFriendRequestAsync(int userId, CancellationToken cancellationToken = default)
    {
        var friendsRequestToRemove = await _socialNetworkDbContext.FriendRequests
            .Where(f => f.SenderId == userId || f.ReceiverId == userId)
            .ToListAsync(cancellationToken);

        if (friendsRequestToRemove.Any())
        {
            _socialNetworkDbContext.FriendRequests.RemoveRange(friendsRequestToRemove);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            foreach (var friendRequest in friendsRequestToRemove)
            {
                await _cacheService.RemoveFromCacheAsync($"FriendRequest-{friendRequest.Id}", cancellationToken);
            }
            
            return true;
        }

        return false;
    }

    public async Task CreateFriendRequestAsync(FriendRequest friendRequest,
        CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.FriendRequests.AddAsync(friendRequest, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);

        await _cacheService.GetOrSetAsync($"FriendRequest-{friendRequest.Id}", (_) => Task.FromResult(friendRequest)!,
            cancellationToken, _socialNetworkDbContext);
    }

    public IQueryable<FriendRequest> GetAllFriendRequests(int id)
    {
        return _socialNetworkDbContext.FriendRequests
            .Include(f => f.Sender.Profile)
            .Include(f => f.Receiver.Profile)
            .Where(f => f.SenderId == id || f.ReceiverId == id);
    }
}