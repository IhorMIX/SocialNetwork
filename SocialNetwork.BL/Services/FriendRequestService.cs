using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class FriendRequestService : IFriendRequestService
{
    private readonly IUserService _userService;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILogger<FriendRequestService> _logger;
    private readonly IMapper _mapper;
    
    public FriendRequestService(IFriendRequestRepository friendRequestRepository, ILogger<FriendRequestService> logger, IMapper mapper, IUserService userService, IFriendshipRepository friendshipRepository)
    {
        _friendRequestRepository = friendRequestRepository;
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _friendshipRepository = friendshipRepository;
    }
    
    public async Task<FriendRequestModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var receiver = await _friendRequestRepository.GetByIdAsync(id, cancellationToken);
        IsExistsHelper.IsExists(receiver, new FriendRequestNotFoundException("Friend request not found"), _logger);
        return _mapper.Map<FriendRequestModel>(receiver);
    }
    
    public async Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken = default)
    {
        var friendRequest = await _friendRequestRepository.GetAll()
            .Where(i => i.ReceiverId == receiverId && i.SenderId == senderId)
            .FirstOrDefaultAsync(cancellationToken);
        
        IsExistsHelper.IsExists(friendRequest, new FriendRequestNotFoundException("Friend request not found"), _logger);
        return _mapper.Map<FriendRequestModel>(friendRequest);
    }
    
    public async Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken = default)
    {
        var senderModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var receiverModel = await _userService.GetByIdAsync(receiverId, cancellationToken);
        
        IsExistsHelper.IsExists(senderModel, new UserNotFoundException("User not found"), _logger);
        IsExistsHelper.IsExists(receiverModel, new UserNotFoundException("User not found"), _logger);
        
        if (senderModel!.Id != receiverModel!.Id)
        {
            await _friendRequestRepository.CreateFriendRequestAsync(new FriendRequest()
            {
                SenderId = senderModel.Id,
                ReceiverId = receiverModel.Id
            }, cancellationToken);
        }
        else
        {
            _logger.LogError("You cant send a friend request to yourself");
        }
    }

    public async Task AcceptRequest(int userId, int senderId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var senderModel = await _userService.GetByIdAsync(senderId, cancellationToken);
        
        IsExistsHelper.IsExists(userModel, new UserNotFoundException("User not found"), _logger);
        IsExistsHelper.IsExists(senderModel, new UserNotFoundException("User not found"), _logger);

        await _friendshipRepository.CreateFriendshipAsync(new Friendship()
        {
            UserId = userModel!.Id,
            FriendId = senderModel!.Id,   
        }, cancellationToken);

        await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
        {
            SenderId = senderModel.Id,
            ReceiverId = userModel.Id
        }, cancellationToken);
    }

    public async Task CancelRequest(int userId, int senderId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var senderModel = await _userService.GetByIdAsync(senderId, cancellationToken);
        
        IsExistsHelper.IsExists(userModel, new UserNotFoundException("User not found"), _logger);
        IsExistsHelper.IsExists(senderModel, new UserNotFoundException("User not found"), _logger);
        
        await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
        {
            SenderId = senderModel!.Id,
            ReceiverId = userModel!.Id
        }, cancellationToken);
    }

    public async Task<IEnumerable<FriendRequestModel>> GetAllRequest(int userId, CancellationToken cancellationToken = default)
    {
        var userRequests = await _friendRequestRepository.GetAll()
            .Include(u=>u.Sender.Profile)
            .Include(u=>u.Receiver.Profile)
            .Where(u => u.ReceiverId == userId)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<FriendRequestModel>>(userRequests);
    }
}