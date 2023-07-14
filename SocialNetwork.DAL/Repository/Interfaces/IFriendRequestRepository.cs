using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IFriendRequestRepository : IBasicRepository<FriendRequest>
{
    Task<bool> DeleteFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken = default);
    Task<bool> DeleteAllFriendRequestAsync(int userId, CancellationToken cancellationToken = default);

    Task CreateFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken = default);
    
    IQueryable<FriendRequest> GetAllFriendRequests(int id);
}