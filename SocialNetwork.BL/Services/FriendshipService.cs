using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILogger<FriendshipService> _logger;
    private readonly IMapper _mapper;

    public FriendshipService(IUserService userService, IUserRepository userRepository, IFriendshipRepository friendshipRepository, ILogger<FriendshipService> logger, IMapper mapper)
    {
        _userService = userService;
        _userRepository = userRepository;
        _friendshipRepository = friendshipRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<FriendshipModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var friendDb = await _friendshipRepository.GetByIdAsync(id, cancellationToken);
        if (friendDb is null)
        {
            _logger.LogError("Friends with this Id {Id} not found", id);
            throw new FriendNotFoundException($"Friends not found");
        }
        
        var friendModel = _mapper.Map<FriendshipModel>(friendDb);
        return friendModel;
    }

    public async Task AddFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId);
        var user2Model = await _userService.GetByIdAsync(firendId);

        if (userModel is null)
        {
            _logger.LogError("User with this Id {Id} not found", userId);
            throw new FriendNotFoundException($"Friends not found");
        }
        if (user2Model is null)
        {
            _logger.LogError("User with this email {firendId} not found", firendId);
            throw new FriendNotFoundException($"Friends not found");
        }

        if (userModel.Id != user2Model.Id)
        {
            var friendship = new FriendshipModel()
            {
                UserId = userModel!.Id,
                FriendId = user2Model!.Id,
                UserModel = _mapper.Map<UserModel>(userModel),
                FriendUserModel = user2Model
            };
            await _friendshipRepository.CreateFriendshipAsync(_mapper.Map<Friendship>(friendship), cancellationToken);
        }
        else
        {
            _logger.LogError("You cant add to friend yourself");
            throw new FriendNotFoundException($"Friends not found");
        }
        
    }
    
    public async Task DeleteFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId);
        var user2Model = await _userService.GetByIdAsync(firendId);
        
        var userDb = await _userRepository.GetByIdAsync(userModel.Id, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Model.Id, cancellationToken);

        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", userModel.Id);
            throw new UserNotFoundException($"User with Id '{userModel.Id}' not found");
        }
        
        if (user2Db is null)
        {
            _logger.LogError("User with this {Id} not found", user2Model.Id);
            throw new UserNotFoundException($"User with Id '{user2Model.Id}' not found");
        }
        var friendship = new FriendshipModel()
        {
            UserId = userModel!.Id,
            FriendId = user2Model!.Id,
            UserModel = userModel,
            FriendUserModel = user2Model
        };
        await _friendshipRepository.DeleteFriendsAsync(_mapper.Map<Friendship>(friendship));
    }

    public async Task<IEnumerable<UserModel>> GetAllFriends(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", userId);
            throw new UserNotFoundException($"User with Id '{userId}' not found");
        }
        var users = await _friendshipRepository.GetAllFriendsAsync(userDb.Id, cancellationToken);
        var userModels = _mapper.Map<IEnumerable<UserModel>>(users);
        return userModels;
    }
    
    public async Task<IEnumerable<UserModel>> FindFriendByNameSurname(int userId, string nameSurname, CancellationToken cancellationToken = default)
    {   
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", userId);
            throw new UserNotFoundException($"User with Id '{userId}' not found");
        }
        
        string[] parts = nameSurname.Split();
        var matchingUsers = await _friendshipRepository.GetAllFriendsAsync(userDb.Id, cancellationToken);
        
        if (parts.Length == 1)
        {
            string name = parts[0].ToLower();
            matchingUsers = matchingUsers.Where(user =>
                user.Profile.Name.ToLower().Contains(name) ||
                user.Profile.Surname.ToLower().Contains(name));
        }
        else if (parts.Length == 2)
        {
            string lastName = parts[0].ToLower();
            string firstName = parts[1].ToLower();
            matchingUsers = matchingUsers.Where(user =>
                user.Profile.Surname.ToLower().Contains(lastName) && user.Profile.Name.ToLower().Contains(firstName) ||
                user.Profile.Name.ToLower().Contains(lastName) && user.Profile.Surname.ToLower().Contains(firstName));
        }
        var friends = _mapper.Map<IEnumerable<UserModel>>(matchingUsers);
        return friends;
    }

    public async Task<UserModel> FindFriendByEmail(int userId, string friendEmail, CancellationToken cancellationToken = default)
    {
        
        var user2Model = await _userService.GetUserByEmail(friendEmail);
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Model.Id, cancellationToken);
        
        if (userDb is null)
        {
            _logger.LogError("User with this {Id} not found", userDb.Id);
            throw new UserNotFoundException($"User with Id '{userDb.Id}' not found");
        }
        if (user2Db is null)
        {
            _logger.LogError("User with this {friendEmail} not found", friendEmail);
            throw new UserNotFoundException($"User with Id '{friendEmail}' not found");
        }
        
        var Friend = _friendshipRepository.GetAll()
            .Where(f => f.UserId == userDb.Id)
            .Select(f => f.FriendUser)
            .FirstOrDefault(u => u.Profile.Email == friendEmail);
        
        if(Friend is null)
            Friend = _friendshipRepository.GetAll()
                .Where(f => f.FriendId == userDb.Id)
                .Select(f => f.User)
                .FirstOrDefault(u => u.Profile.Email == friendEmail);
        
        if (Friend is null)
        {
            _logger.LogError("User with this {friendEmail} is not a friend",friendEmail);
            throw new UserNotFoundException($"User with Id '{friendEmail}' is not a friend");
        }
        
        var userModel = _mapper.Map<UserModel>(Friend);
        return userModel;
    }
}