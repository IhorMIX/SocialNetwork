using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IUserService : IBaseService<UserModel>
{
    Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateUserAsync(int id, UserModel user, CancellationToken cancellationToken = default);
    
    Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByEmail(string email, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByLogin(string login, CancellationToken cancellationToken = default);
    
    Task AddAuthorizationValueAsync(UserModel user, string refreshToken, LoginType loginType, DateTime? expiredDate = null,
        CancellationToken cancellationToken = default);

    Task LogOutAsync(int userId, CancellationToken cancellationToken = default);

    Task ActivateUser(int userId, CancellationToken cancellationToken = default);

    Task ChangeOnlineStatus(int userId, CancellationToken cancellationToken = default);
    Task ResetPasswordConfirmationAsync(string userEmail, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default);

}