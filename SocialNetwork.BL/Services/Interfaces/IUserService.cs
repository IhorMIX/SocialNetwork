using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IUserService : IBaseService<UserModel>
{
    Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(UserModel user, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateRefreshTokenAsync(int id, string refreshToken, CancellationToken cancellationToken = default);

    Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password, CancellationToken cancellationToken = default);
}