using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public RoleRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
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
        return await _socialNetworkDbContext.Roles
            .Include(r => r.ChatMembers)
            .ThenInclude(cm => cm.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role> CreateRole(Role role, CancellationToken cancellationToken = default)
    {
       await _socialNetworkDbContext.Roles.AddAsync(role, cancellationToken);
       await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
       return role;
    }

    public async Task DeleteRole(Role role, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.Remove(role);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EditRole(Role role, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.Update(role);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task EditRole(List<Role> role, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Roles.UpdateRange(role);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}