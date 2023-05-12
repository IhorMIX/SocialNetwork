using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IUserRepository : IBasicRepository<User>
{
    Task CreateUser(User user, CancellationToken cancellationToken = default);

    Task<User?> FindUserAsync(string login, CancellationToken cancellationToken = default);

    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(User user, CancellationToken cancellationToken = default);
}