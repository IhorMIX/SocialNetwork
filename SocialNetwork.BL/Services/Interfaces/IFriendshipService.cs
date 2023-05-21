using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IFriendshipService : IBaseService<FriendshipModel>
{
    Task AddFriendshipAsync(int userId, string friendEmail, CancellationToken cancellationToken = default);
    Task DeleteFriendshipAsync(UserModel userModel, UserModel user2Model, CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetFriendship(UserModel userModel, CancellationToken cancellationToken = default);
    
    // we can pass NameSurname like "Name Surname", "Surname Name", "Name", "Surname" and find all who has something similar
    Task<IEnumerable<User>> FindFriendByNameSurname(UserModel userModel, string nameSurname, CancellationToken cancellationToken = default);
    
    Task<UserModel> FindFriendByEmail(UserModel? usermodel, string email, CancellationToken cancellationToken = default);
}