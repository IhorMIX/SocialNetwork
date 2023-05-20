using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IFriendshipRepository : IBasicRepository<Friendship>
{
    Task<bool> DeleteFriendsAsync(Friendship friendship, CancellationToken cancellationToken = default);

    Task CreateFriendshipAsync(Friendship friendship, CancellationToken cancellationToken = default);
    
    Task<Friendship?> GetByFriendIdsAsync(Friendship friendship, CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetAllFriendsAsync(int id, CancellationToken cancellationToken = default);
    
}