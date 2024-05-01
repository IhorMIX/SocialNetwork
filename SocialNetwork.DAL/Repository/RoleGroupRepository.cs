using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository
{
    public class RoleGroupRepository : IRoleGroupRepository
    {
        private readonly SocialNetworkDbContext _socialNetworkDbContext;

        public RoleGroupRepository(SocialNetworkDbContext socialNetworkDbContext)
        {
            _socialNetworkDbContext = socialNetworkDbContext;
        }

        public IQueryable<RoleGroup> GetAll()
        {
            return _socialNetworkDbContext.RoleGroups
                .Include(r => r.RoleAccesses)
                .Include(i => i.Group)
                .Include(i => i.GroupMembers)
                .ThenInclude(cm => cm.User)
                .AsQueryable();
        }
        public async Task<RoleGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.RoleGroups
                .Include(r => r.RoleAccesses)
                .Include(r => r.GroupMembers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<RoleGroup> CreateRole(RoleGroup roleGroup, CancellationToken cancellationToken = default)
        {
            await _socialNetworkDbContext.RoleGroups.AddAsync(roleGroup, cancellationToken);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
            return roleGroup;
        }

        public async Task DeleteRole(RoleGroup roleGroup, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.RoleGroups.Remove(roleGroup);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task EditRole(RoleGroup roleGroup, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.RoleGroups.Update(roleGroup);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task EditRole(List<RoleGroup> roleGroup, CancellationToken cancellationToken = default)
        {
            _socialNetworkDbContext.RoleGroups.UpdateRange(roleGroup);
            await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<RoleGroup?> GetByName(string name, CancellationToken cancellationToken = default)
        {
            return await _socialNetworkDbContext.RoleGroups
                .Include(r => r.GroupMembers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(r => r.RoleName == name, cancellationToken);
        }
    }
}