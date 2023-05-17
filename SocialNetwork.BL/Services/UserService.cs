using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using System.Data;
using SocialNetwork.BL.Models.Enums;
using Profile = SocialNetwork.DAL.Entity.Profile;

namespace SocialNetwork.BL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<UserModel?> GetById(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(id, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with this Id {Id} not found", id);
            throw new UserNotFoundException($"User with Id '{id}' not found");
        }

        var userModel = _mapper.Map<UserModel>(user);
        return userModel;
    }

    public async Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDbModel = _mapper.Map<User>(user);

        userDbModel.Password = PasswordHelper.HashPassword(userDbModel.Password);

        await _userRepository.CreateUser(userDbModel, cancellationToken);
    }

    public async Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(user.Id, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", user.Id);
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }

        userDb.Password = string.IsNullOrEmpty(user.Password)
            ? userDb.Password
            : PasswordHelper.HashPassword(user.Password);
        userDb.Login = string.IsNullOrEmpty(user.Login) ? userDb.Login : user.Login;

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<ProfileModel, Profile>.PropertyMap)
        {
            var userProperty = propertyMap.Item1;
            var userDbProperty = propertyMap.Item2;

            var userSourceValue = userProperty.GetValue(user.Profile);
            var userTargetValue = userDbProperty.GetValue(userDb.Profile);

            if (userSourceValue != null && !userSourceValue.Equals(userTargetValue))
            {
                userDbProperty.SetValue(userDb.Profile, userSourceValue);
            }
        }

        await _userRepository.UpdateUserAsync(userDb, cancellationToken);
        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task DeleteUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(user.Id, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", user.Id);
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }

        await _userRepository.DeleteUserAsync(userDb, cancellationToken);
    }

    public async Task AddAuthorizationValueAsync(UserModel user, string refreshToken, LoginType loginType, DateTime? expiredDate = null,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(user.Id, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", user.Id);
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }
            
        if (userDb.AuthorizationInfo is not null)
        {
            userDb.AuthorizationInfo.RefreshToken = refreshToken;
            userDb.AuthorizationInfo.ExpiredDate = expiredDate;
            userDb.AuthorizationInfo.LoginType = (DAL.Entity.Enums.LoginType)loginType;
        }
        else
        {
            userDb.AuthorizationInfo = new AuthorizationInfo
            {
                RefreshToken = refreshToken,
                ExpiredDate = expiredDate,
                LoginType = (DAL.Entity.Enums.LoginType)loginType
            };
        }
        await _userRepository.UpdateUserAsync(userDb, cancellationToken);
    }
    
    public async Task LogOutAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetById(userId, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this token not found");
            throw new UserNotFoundException($"User with this token not found");
        }
            
        if (userDb.AuthorizationInfo is not null)
        {
            userDb.AuthorizationInfo = null;
            await _userRepository.UpdateUserAsync(userDb, cancellationToken);
        }
        else throw new NullReferenceException($"User with this token not found");
    }

    public async Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstOrDefaultAsync(i => i.Login == login, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this Login {Login} not found", login);
            throw new UserNotFoundException($"User not found");
        }

        if (!PasswordHelper.VerifyHashedPassword(userDb.Password, password))
        {
            throw new WrongLoginOrPasswordException("Wrong login or password");
        }

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll()
            .FirstOrDefaultAsync(i => i.AuthorizationInfo != null && i.AuthorizationInfo.RefreshToken == refreshToken, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("refresh token not found");
            throw new UserNotFoundException($"User not found");
        }

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Profile.Email == email, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this Login {Email} not found", email);
            throw new UserNotFoundException($"User not found");
        }


        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel?> GetUserByLogin(string login, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstOrDefaultAsync(i => i.Login == login, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this Login {Login} not found", login);
            throw new UserNotFoundException($"User not found");
        }


        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }
}