using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using System.Reflection.Metadata.Ecma335;
using SocialNetwork.DAL.Services;

namespace SocialNetwork.DAL.Repository;

public class UserRepository : IUserRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly CacheService<User?> _cacheService;

    public UserRepository(SocialNetworkDbContext socialNetworkDbContext, IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository, CacheService<User?> cacheService)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _friendshipRepository = friendshipRepository;
        _friendRequestRepository = friendRequestRepository;
        _cacheService = cacheService;
    }

    public IQueryable<User> GetAll()
    {
        return _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo).AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // return await _cacheService.GetOrSetAsync($"User-{id}", async (token) =>
        // {
        //     return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
        //         .FirstOrDefaultAsync(i => i.Id == id && i.IsEnabled, token);
        // }, cancellationToken);
        
        return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
                .FirstOrDefaultAsync(i => i.Id == id && i.IsEnabled, cancellationToken);
    }


    public async Task CreateUser(User user, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Users.AddAsync(user, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);

        await _cacheService.GetOrSetAsync($"User-{user.Id}", (_) => Task.FromResult(user)!, cancellationToken, _socialNetworkDbContext);
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

        await _cacheService.UpdateAsync($"User-{user.Id}", (_) => Task.FromResult(user)!, cancellationToken);
    }

    public async Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _friendshipRepository.DeleteAllFriendsAsync(user.Id, cancellationToken);
        await _friendRequestRepository.DeleteAllFriendRequestAsync(user.Id, cancellationToken);

        _socialNetworkDbContext.Users.Remove(user);

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveFromCacheAsync($"User-{user.Id}", cancellationToken);
    }

    public Task<User?> GetByIdDisabledUser(int id, CancellationToken cancellationToken = default)
    {
        return _socialNetworkDbContext.Users.FirstOrDefaultAsync(i => i.Id == id && !i.IsEnabled, cancellationToken);
    }
}