using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class UserRepository : IUserRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendRequestRepository _friendRequestRepository;

    public UserRepository(SocialNetworkDbContext socialNetworkDbContext, IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _friendshipRepository = friendshipRepository;
        _friendRequestRepository = friendRequestRepository;
    }

    public IQueryable<User> GetAll()
    {
        return _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo).AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
                .FirstOrDefaultAsync(i => i.Id == id && i.IsEnabled, cancellationToken);
    }


    public Task<List<User>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default)
    {
        return _socialNetworkDbContext.Users.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
    }

    public async Task CreateUser(User user, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Users.AddAsync(user, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> FindUserAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Users
            .Include(i => i.Profile)
            .Include(i => i.AuthorizationInfo)
            .FirstOrDefaultAsync(i => i.Login == login && i.IsEnabled, cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Users.Update(user);

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _friendshipRepository.DeleteAllFriendsAsync(user.Id, cancellationToken);
        await _friendRequestRepository.DeleteAllFriendRequestAsync(user.Id, cancellationToken);

        _socialNetworkDbContext.Users.Remove(user);

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByIdDisabledUser(int id, CancellationToken cancellationToken = default)
    {
        return _socialNetworkDbContext.Users.FirstOrDefaultAsync(i => i.Id == id && !i.IsEnabled, cancellationToken);
    }

    public async Task ChangeOnlineStatus(int userId, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(r => r.SetProperty(b => b.OnlineStatus, 
                b => b.OnlineStatus == OnlineStatus.Online ? OnlineStatus.Offline : OnlineStatus.Online), 
            cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}