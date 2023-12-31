using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public FriendRequestRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
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
        return await _socialNetworkDbContext.FriendRequests
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
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
            
            return true;
        }

        return false;
    }

    public async Task<int> CreateFriendRequestAsync(FriendRequest friendRequest,
        CancellationToken cancellationToken = default)
    {
        
        var entityEntry = await _socialNetworkDbContext.FriendRequests.AddAsync(friendRequest, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity.Id;
    }

    public async Task<bool> RequestExists(int senderId, int receiverId, CancellationToken cancellationToken = default)
    {
        var result = await _socialNetworkDbContext.FriendRequests
            .SingleOrDefaultAsync(fr => (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                                        (fr.SenderId == receiverId && fr.ReceiverId == senderId),
                                        cancellationToken);
        return result != null;
    }

    public IQueryable<FriendRequest> GetAllFriendRequests(int id)
    {
        return _socialNetworkDbContext.FriendRequests
            .Include(f => f.Sender.Profile)
            .Include(f => f.Receiver.Profile)
            .Where(f => f.SenderId == id || f.ReceiverId == id);
    }
}