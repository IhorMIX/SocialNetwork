using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class FriendRequestService : IFriendReqestService
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
        //if
        return _mapper.Map<FriendRequestModel>(receiver);
    }

    public async Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken)
    {
        var senderModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var receiverModel = await _userService.GetByIdAsync(receiverId, cancellationToken);
        
        //if
        
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

    public async Task AcceptRequest(int userId, int friendRequestId, CancellationToken cancellationToken)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var requestDb = await _friendRequestRepository.GetByIdAsync(friendRequestId, cancellationToken);
        //if
        
        await _friendshipRepository.CreateFriendshipAsync(new Friendship()
        {
            UserId = userModel!.Id,
            FriendId = requestDb!.SenderId,   
        }, cancellationToken);
    }

    public async Task CancelRequest(int userId, int friendRequestId, CancellationToken cancellationToken)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var requestDb = await _friendRequestRepository.GetByIdAsync(friendRequestId, cancellationToken);
        //if
        
        await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
        {
            SenderId = requestDb.SenderId,
            ReceiverId = userModel.Id
        });
    }

    public async Task<IEnumerable<FriendRequestModel>> GetAllRequest(int userId, CancellationToken cancellationToken)
    {
        var userRequests = await _friendRequestRepository.GetAll()
            .Where(u => u.ReceiverId == userId)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<FriendRequestModel>>(userRequests);
    }
}