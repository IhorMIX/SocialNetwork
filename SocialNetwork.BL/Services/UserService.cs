using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Settings;
using Profile = SocialNetwork.DAL.Entity.Profile;

namespace SocialNetwork.BL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IMailService _mailService;
    private readonly TemplatePatheOptions _templatePatheOptions;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper,
        IMailService mailService, IOptions<TemplatePatheOptions> templatePatheOptions)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
        _mailService = mailService;
        _templatePatheOptions = templatePatheOptions.Value;
    }

    public async Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {id} not found"));
        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task CreateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Login == user.Login || i.Profile.Email == user.Profile.Email,
                cancellationToken);

        if (userDb != null)
        {
            if (userDb.Login == user.Login)
                throw new AlreadyLoginAndEmailException("Login is already used by another user");

            if (userDb.Profile.Email == user.Profile.Email)
                throw new AlreadyLoginAndEmailException("Email is already used by another user");
        }

        var userDbModel = _mapper.Map<User>(user);

        userDbModel.Password = PasswordHelper.HashPassword(userDbModel.Password);

        await _userRepository.CreateUser(userDbModel, cancellationToken);

        user.Id = userDbModel.Id;

        await _mailService.SendHtmlEmailAsync(new MailModel()
        {
            Subject = "Mail Confirmation",
            Data = user.ToScriptObject(),
            EmailTo = user.Profile.Email,
            FilePath = _templatePatheOptions.MailActivation
        });
    }

    public async Task<UserModel> UpdateUserAsync(int id, UserModel user, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(id, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {id} not found"));

        userDb!.Password = string.IsNullOrEmpty(user.Password)
            ? userDb.Password
            : PasswordHelper.HashPassword(user.Password);

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<ProfileModel, Profile>.PropertyMap)
        {
            var userProperty = propertyMap.Item1;
            var userDbProperty = propertyMap.Item2;

            var userSourceValue = userProperty.GetValue(user.Profile);
            var userTargetValue = userDbProperty.GetValue(userDb.Profile);

            if (userSourceValue != null && userSourceValue != "" && !userSourceValue.Equals(userTargetValue))
            {
                userDbProperty.SetValue(userDb.Profile, userSourceValue);
            }
        }

        await _userRepository.UpdateUserAsync(userDb, cancellationToken);
        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(id, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {id} not found"));

        await _userRepository.DeleteUserAsync(userDb!, cancellationToken);
    }

    public async Task AddAuthorizationValueAsync(UserModel user, string refreshToken, LoginType loginType,
        DateTime? expiredDate = null,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {user.Id} not found"));

        if (userDb.AuthorizationInfo is not null && userDb.AuthorizationInfo.ExpiredDate <= DateTime.Now.AddDays(-1))
            await LogOutAsync(user.Id, cancellationToken);

        userDb.AuthorizationInfo = new AuthorizationInfo
        {
            RefreshToken = refreshToken,
            ExpiredDate = expiredDate,
            LoginType = (DAL.Entity.Enums.LoginType)loginType
        };
        await _userRepository.UpdateUserAsync(userDb, cancellationToken);
    }

    public async Task LogOutAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        if (userDb.AuthorizationInfo is not null)
        {
            userDb.AuthorizationInfo = null;
            await _userRepository.UpdateUserAsync(userDb, cancellationToken);
        }
        else throw new NullReferenceException($"User with this token not found");
    }

    public async Task ActivateUser(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdDisabledUser(userId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb,
            new UserNotFoundException($"You have already activated account or User with this Id {userId} not found!"));

        userDb!.IsEnabled = true;

        await _userRepository.UpdateUserAsync(userDb, cancellationToken);

        _logger.LogInformation("User activated");
    }

    public async Task<UserModel?> GetUserByLoginAndPasswordAsync(string login, string password,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll().FirstOrDefaultAsync(i => i.Login == login && i.IsEnabled, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this login {login} not found"));

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
            .FirstOrDefaultAsync(i =>
                    i.AuthorizationInfo != null &&
                    i.AuthorizationInfo.RefreshToken == refreshToken
                    && i.IsEnabled,
                cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb,
            new UserNotFoundException($"User with this refresh token {refreshToken} not found"));

        if (userDb.AuthorizationInfo is not null && userDb.AuthorizationInfo.ExpiredDate <= DateTime.Now.AddDays(-1))
            throw new TimeoutException();

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Profile.Email == email && i.IsEnabled, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this email {email} not found"));

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }

    public async Task<UserModel?> GetUserByLogin(string login, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Login == login && i.IsEnabled, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this login {login} not found"));

        var userModel = _mapper.Map<UserModel>(userDb);
        return userModel;
    }
}