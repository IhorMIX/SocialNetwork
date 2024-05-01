using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository.Interfaces
{
    public interface IRoleGroupRepository : IBasicRepository<RoleGroup>
    {
        public Task<RoleGroup> CreateRole(RoleGroup roleGroup, CancellationToken cancellationToken = default);
        public Task DeleteRole(RoleGroup roleGroup, CancellationToken cancellationToken = default);
        public Task EditRole(RoleGroup roleGroup, CancellationToken cancellationToken = default);
        public Task EditRole(List<RoleGroup> roleGroup, CancellationToken cancellationToken = default);
        public Task<RoleGroup?> GetByName(string name, CancellationToken cancellationToken = default);
    }
}
