using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class UserRepository : IUserRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public UserRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<User> GetAll()
    {
        return _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo).AsQueryable();
    }

    public async Task<User?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task CreateUser(User user, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Users.AddAsync(user, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> FindUserAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Users.Include(i => i.Profile).Include(i => i.AuthorizationInfo)
            .FirstOrDefaultAsync(i => i.Login == login, cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Users.Update(user);

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Users.Remove(user);

        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}