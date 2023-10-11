using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IRoleRepository : IBasicRepository<Role>
{
    public Task<Role> CreateRole(Role role, CancellationToken cancellationToken = default);
    public Task DeleteRole(Role role, CancellationToken cancellationToken = default);
    public Task EditRole(Role role, CancellationToken cancellationToken = default);
    public Task EditRole(List<Role> roles, CancellationToken cancellationToken = default);
    
    public Task<Role?> GetByName(string name, CancellationToken cancellationToken = default);
}