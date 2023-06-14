using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IFriendshipRepository : IBasicRepository<Friendship>
{
    Task<bool> DeleteFriendsAsync(Friendship friendship, CancellationToken cancellationToken = default);

    Task CreateFriendshipAsync(Friendship friendship, CancellationToken cancellationToken = default);
    
    IQueryable<Friendship> GetAllFriends(int id);
}