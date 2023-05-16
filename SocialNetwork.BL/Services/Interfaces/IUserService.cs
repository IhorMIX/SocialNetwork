using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IUserService : IBaseService<UserModel>
{
    Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(UserModel user, CancellationToken cancellationToken = default);
    
    Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByEmail(string email, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByLogin(string login, CancellationToken cancellationToken = default);
    
    Task AddAuthorizationValueAsync(UserModel user, string token, LoginType loginType, DateTime? expiredDate = null,
        CancellationToken cancellationToken = default);
}