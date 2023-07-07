using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IFriendshipService : IBaseService<FriendshipModel>
{
    Task AddFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default);
    Task DeleteFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserModel>> GetAllFriends(int userId, CancellationToken cancellationToken = default);
    
    // we can pass NameSurname like "Name Surname", "Surname Name", "Name", "Surname" and find all who has something similar
    Task<IEnumerable<UserModel>> FindFriendByNameSurname(int userId, string nameSurname, CancellationToken cancellationToken = default);
    
    Task<UserModel> FindFriendByEmail(int userId, string friendEmail, CancellationToken cancellationToken = default);
}