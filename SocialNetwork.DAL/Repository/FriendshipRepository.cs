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
    
    public async Task<IEnumerable<User>> GetAllFriendsAsync(int id, CancellationToken cancellationToken = default)
    {
        var userFriends = await _socialNetworkDbContext.Friends
            .Include(f => f.FriendUser)
            .Where(f => f.UserId == id)
            .Select(f => f.FriendUser)
            .ToListAsync(cancellationToken);
        
        // only userFriends check!!!!!!!!!!!!!
        var friendFriends = await _socialNetworkDbContext.Friends
            .Include(f => f.User)
            .Where(f => f.FriendId == id)
            .Select(f => f.User)
            .ToListAsync(cancellationToken);
        
        var allFriends = userFriends.Concat(friendFriends).ToList();
        return allFriends;

        //_socialNetworkDbContext.Users.Include(i => i.Friends);

        // var combinedFriends = await _socialNetworkDbContext.Friends
        //     .Where(f => f.UserId == id || f.FriendId == id)
        //     .Select(f => f.UserId == id ? f.FriendUser : f.User)
        //     .ToListAsync(cancellationToken);
        // return combinedFriends;
    }
    
    public async Task<bool> DeleteFriendsAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        
        //looking for all records that they are friends
        var friendsToRemove = await _socialNetworkDbContext.Friends
            .Where(f =>
                (f.UserId == friendship.UserId && f.FriendId == friendship.FriendId) ||
                (f.UserId == friendship.FriendId && f.FriendId == friendship.UserId))
            .ToListAsync(cancellationToken);

        //deleting this records if we found somth
        if(friendsToRemove.Count > 0)
        {
            _socialNetworkDbContext.Friends.RemoveRange(friendsToRemove);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}