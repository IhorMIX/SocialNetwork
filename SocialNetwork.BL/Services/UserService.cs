using Microsoft.EntityFrameworkCore;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserModel?> GetById(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(id, cancellationToken);
        
        if (user is null)
        {
            throw new UserNotFoundException($"User with Id '{id}' not found");
        }

        return UserMapper.ConvertUserToBlModel(user);
    }

    public async Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDbModel = UserMapper.ConvertUserToDalModel(user);

        userDbModel.Password = PasswordHelper.HashPassword(userDbModel.Password);

        await _userRepository.CreateUser(userDbModel, cancellationToken);
    }

    public async Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(user.Id, cancellationToken);
        
        if (userDb is null)
        {
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }
        
        //TODO:Needs to update fields. I will do this next time

        await _userRepository.UpdateUserAsync(userDb, cancellationToken);

        return UserMapper.ConvertUserToBlModel(userDb)!;
    }

    public async Task DeleteUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(user.Id, cancellationToken);

        if (userDb is null)
        {
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }

        await _userRepository.DeleteUserAsync(userDb, cancellationToken);
    }

    public async Task<UserModel> UpdateRefreshTokenAsync(int id, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(id, cancellationToken);

        if (userDb is null)
        {
            throw new UserNotFoundException($"User with Id '{id}' not found");
        }

        userDb.AuthorizationInfo.RefreshToken = refreshToken;
        await _userRepository.UpdateUserAsync(userDb, cancellationToken);

        return UserMapper.ConvertUserToBlModel(userDb)!;
    }
    
    public async Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstOrDefaultAsync(i => i.Login == login, cancellationToken);
        
        if (userDb is null)
        {
            throw new UserNotFoundException($"User not found");
        }

        if (!PasswordHelper.VerifyHashedPassword(userDb.Password, password))
        {
            throw new UserNotFoundException($"User not found");
        }

        return UserMapper.ConvertUserToBlModel(userDb)!;
    }

    public async Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstOrDefaultAsync(i => i.AuthorizationInfo.RefreshToken == refreshToken, cancellationToken);
        
        if (userDb is null)
        {
            throw new UserNotFoundException($"User not found");
        }

        return UserMapper.ConvertUserToBlModel(userDb)!;
    }
}