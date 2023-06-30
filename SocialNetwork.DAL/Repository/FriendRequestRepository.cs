using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class FriendRequestRepository : IFriendRequestRepository
{
    public IQueryable<FriendRequest> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<FriendRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}