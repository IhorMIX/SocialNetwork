using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Repository.Interfaces;
using System.Data;

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
            throw new UserNotFoundException($"User with Id '{user.Id}' not found");
        }

        userDb.Password = string.IsNullOrEmpty(user.Password) ? userDb.Password : PasswordHelper.HashPassword(user.Password); 
        userDb.Login = string.IsNullOrEmpty(user.Login) ? userDb.Login : user.Login;
        //userDb.Profile.Birthday = (user.Profile.Birthday == DateTime.MinValue) ? userDb.Profile.Birthday : user.Profile.Birthday;
        // userDb.Profile.Name = string.IsNullOrEmpty(user.Profile.Name) ? userDb.Profile.Name : user.Profile.Name;
        /// userDb.Profile.Surname = string.IsNullOrEmpty(user.Profile.Surname) ? userDb.Profile.Surname : user.Profile.Surname;
        // userDb.Profile.Email = string.IsNullOrEmpty(user.Profile.Email) ? userDb.Profile.Email : user.Profile.Email;
        //userDb.Profile.AvatarImage = string.IsNullOrEmpty(user.Profile.AvatarImage) ? userDb.Profile.AvatarImage : user.Profile.AvatarImage;
        // userDb.Profile.Description = string.IsNullOrEmpty(user.Profile.Description) ? userDb.Profile.Description : user.Profile.Description;
        //userDb.Profile.Sex =;
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<UserModel, ProfileModel>.PropertyMap)
        {
            var sourceProperty = propertyMap.Item1;
            var targetProperty = propertyMap.Item2;

            var sourceValue = sourceProperty.GetValue(user.Profile);
            var targetValue = targetProperty.GetValue(userDb.Profile);

            if (sourceValue != null && !sourceValue.Equals(targetValue))
            {
                targetProperty.SetValue(userDb.Profile, sourceValue);
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

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }
    
    public async Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstAsync(i => i.Login == login && i.Password == PasswordHelper.HashPassword(password), cancellationToken);
        
        if (userDb is null)
        {
            _logger.LogError("User with this Login {login} not found", login);
            throw new UserNotFoundException($"User not found");
        }

        if (!PasswordHelper.VerifyHashedPassword(userDb.Password, password))
        {
            throw new WrongPasswordException("Wrong login or password");
        }
        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstAsync(i => i.AuthorizationInfo.RefreshToken == refreshToken, cancellationToken);
        
        if (userDb is null)
        {
            throw new UserNotFoundException($"User not found");
        }
        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }
}