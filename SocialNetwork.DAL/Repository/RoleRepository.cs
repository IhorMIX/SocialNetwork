using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Services;

namespace SocialNetwork.DAL.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly CacheService<Role?> _cacheService;
    public RoleRepository(SocialNetworkDbContext socialNetworkDbContext, CacheService<Role?> cacheService)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _cacheService = cacheService;
    }
    
    public IQueryable<Role> GetAll()
    {
        return _socialNetworkDbContext.Roles
            .Include(i=>i.Chat)
            .Include(i=>i.ChatMembers)
            .ThenInclude(cm => cm.User)
            .AsQueryable();
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        
        return await _cacheService.GetOrSetAsync($"Role - {id}", async (token) =>
        {
            return await _socialNetworkDbContext.Roles
                .Include(r => r.ChatMembers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(r => r.Id == id, token);
        }, cancellationToken);
        
       
    }

    public async Task<Role> CreateRole(Role role, CancellationToken cancellationToken = default)
    {
       await _socialNetworkDbContext.Roles.AddAsync(role, cancellationToken);
       await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
       await _cacheService.GetOrSetAsync($"Role-{role.Id}", (_) => Task.FromResult(role)!, cancellationToken);
       return role;
    }

    public async Task DeleteRole(Role role, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.Remove(role);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveFromCacheAsync($"Role-{role.Id}", cancellationToken);
    }

    public async Task EditRole(Role role, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.Update(role);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.UpdateAsync($"Role-{role.Id}", (_) => Task.FromResult(role)!, cancellationToken);
    }
    public async Task EditRole(List<Role> roles, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.UpdateRange(roles);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);

        foreach (var role in roles)
            await _cacheService.UpdateAsync($"Role-{role.Id}", (_) => Task.FromResult(role)!, cancellationToken);
    }
}