using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public FriendshipRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }
    
    public IQueryable<Friendship> GetAll()
    {
        return _socialNetworkDbContext.Friends
            .Include(i => i.FriendUser)
            .Include(i => i.User)
            .AsQueryable();
    }

    public async Task<Friendship?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Friends
            .Include(i => i.FriendUser)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task CreateFriendshipAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Friends.AddAsync(friendship, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> DeleteFriendsAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        var friendToRemove = await _socialNetworkDbContext.Friends
            .Where(f =>
                (f.UserId == friendship.UserId && f.FriendId == friendship.FriendId) ||
                (f.UserId == friendship.FriendId && f.FriendId == friendship.UserId))
            .SingleOrDefaultAsync(cancellationToken);
        
        if(friendToRemove is not null)
        {
            _socialNetworkDbContext.Friends.Remove(friendToRemove);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
    public async Task<bool> DeleteAllFriendsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var friendsToRemove = await _socialNetworkDbContext.Friends
            .Where(f => f.UserId == userId || f.FriendId == userId)
            .ToListAsync(cancellationToken);
        
        if(friendsToRemove.Any())
        {
            _socialNetworkDbContext.Friends.RemoveRange(friendsToRemove);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
    
    public IQueryable<Friendship> GetAllFriendsByUserId(int id)
    {
        var query = _socialNetworkDbContext.Friends
            .Include(f => f.User.Profile)
            .Include(f => f.FriendUser.Profile)
            .Where(f => f.UserId == id || f.FriendId == id);
    
        return query;
    }

}