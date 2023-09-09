using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace SocialNetwork.DAL.Repository;

public class UserRepository : IUserRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly CachingHelper<User?> _cachingHelper;
    public UserRepository(SocialNetworkDbContext socialNetworkDbContext, IFriendshipRepository friendshipRepository, IFriendRequestRepository friendRequestRepository, CachingHelper<User?> cachingHelper)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _friendshipRepository = friendshipRepository;
        _friendRequestRepository = friendRequestRepository;
        _cachingHelper = cachingHelper;

    }

    public IQueryable<User> GetAll()
    {
        return _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo).AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cachingHelper.GetOrUpdate($"User-{id}", async (token) =>
        {
            return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
            .FirstOrDefaultAsync(i => i.Id == id, token);
        }, cancellationToken);
        
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
                .FirstOrDefaultAsync(i => i.Login == login, cancellationToken);

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
}