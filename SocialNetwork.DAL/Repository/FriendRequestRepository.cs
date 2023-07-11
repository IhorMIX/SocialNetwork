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

    public async Task<bool> DeleteFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken = default)
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
    
    public async Task CreateFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.FriendRequests.AddAsync(friendRequest, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<FriendRequest> GetAllFriendRequests(int id)
    {
        return _socialNetworkDbContext.FriendRequests
            .Include(f => f.Sender.Profile)
            .Include(f => f.Receiver.Profile)
            .Where(f => f.SenderId == id || f.ReceiverId == id);
    }
}