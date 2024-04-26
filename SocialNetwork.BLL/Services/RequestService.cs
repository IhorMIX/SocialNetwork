using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;


namespace SocialNetwork.BLL.Services;

public class RequestService : IRequestService
{
    private readonly IUserService _userService;
    private readonly IRequestRepository _requestRepository;
    private readonly ILogger<RequestService> _logger;
    private readonly IMapper _mapper;
    private readonly IBlackListService _blackListService;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendshipService _friendshipService;
    private readonly IGroupRepository _groupRepository;
    private readonly IBannedUserListRepository _bannedUserListRepository;
    private readonly IGroupService _groupService;
    private readonly IGroupMemberRepository _groupMemberRepository;

    public RequestService(IRequestRepository requestRepository, ILogger<RequestService> logger, IMapper mapper, IUserService userService,
        IBlackListService blackListService, IFriendshipRepository friendshipRepository, IGroupRepository groupRepository,
        IFriendshipService friendshipService, IBannedUserListRepository bannedUserListRepository, IGroupService groupService, IGroupMemberRepository groupMemberRepository)
    {
        _requestRepository = requestRepository;
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _blackListService = blackListService;
        _friendshipRepository = friendshipRepository;
        _groupRepository = groupRepository;
        _friendshipService = friendshipService;
        _bannedUserListRepository = bannedUserListRepository;
        _groupService = groupService;
        _groupMemberRepository = groupMemberRepository;

    }

    public async Task<BaseRequestModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(request, new RequestException($"Request with this Id {id} not found"));
        return _mapper.Map<BaseRequestModel>(request);
    }
    public async Task<int> SendFriendRequestAsync(FriendRequestModel friendRequestModel, CancellationToken cancellationToken = default)
    {
        var senderModel = await _userService.GetByIdAsync(friendRequestModel.SenderId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(senderModel, new UserNotFoundException($"User with ID {friendRequestModel.SenderId} not found."));

        var receiverModel = await _userService.GetByIdAsync(friendRequestModel.ToUserId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(receiverModel, new UserNotFoundException($"User with ID {friendRequestModel.ToUserId} not found."));

        if (await _friendshipService.IsFriends(senderModel!.Id, receiverModel!.Id))
        {
            _logger.LogError("You can't send a friend request to your friend");
            throw new FriendRequestException("You can't send a friend request to your friend");
        }

        var requestExists = await _requestRepository.RequestExists(new FriendRequest() { SenderId = senderModel!.Id, ToUserId = receiverModel!.Id }, cancellationToken);
        if (requestExists is true)
        {
            _logger.LogError("Friend request already exists");
            throw new RequestException("Friend request already exists");
        }


        if (senderModel!.Id != receiverModel!.Id)
        {
            if (await _blackListService.IsBannedUser(senderModel.Id, receiverModel.Id, cancellationToken))
            {
                _logger.LogError("You can't send a friend request to a banned user");
                throw new FriendRequestException("Friend request can't be sent to a banned user");
            }

            if (await _blackListService.IsBannedUser(receiverModel.Id, senderModel.Id, cancellationToken))
            {
                _logger.LogError("You can't send a friend request to a user who banned you");
                throw new FriendRequestException("Friend request can't be sent to a user who banned you");
            }

            return await _requestRepository.CreateRequestAsync(new FriendRequest()
            {
                SenderId = senderModel!.Id,
                ToUserId = receiverModel!.Id,
                CreatedAt = DateTime.Now,
            }, cancellationToken);

            //TODO:Notification
            // in notification box
            //return await _notificationRepository.CreateNotification(new FriendRequestNotification()
            //{
            //    NotificationMessage =
            //        $"Friend request from {senderModel.Profile.Name} {senderModel.Profile.Surname}",
            //    CreatedAt = DateTime.Now,
            //    IsRead = false,
            //    ToUserId = receiverModel.Id,
            //    FriendRequestId = friendRequestId,
            //    InitiatorId = senderModel.Id,
            //}, cancellationToken);
        }
        else
        {
            _logger.LogError("You can't send a friend request to yourself");
            throw new FriendRequestException("Friend request can't be sent to yourself");
        }

    }

    public async Task AcceptFriendRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var friendRequest = await _requestRepository.GetByIdAsync(requestId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(friendRequest, new FriendRequestException($"Friend request by id {requestId} not found"));

        if (friendRequest is FriendRequest f)
        {
            var friends = await _friendshipRepository
        .GetAllFriendsByUserId(userId)
        .Where(f => f.UserId == userId && f.FriendId == ((FriendRequest)friendRequest)!.ToUserId ||
                    f.UserId == (friendRequest as FriendRequest)!.ToUserId && f.FriendId == userId)
        .Select(f => f.UserId == userId ? f.FriendUser : f.User)
        .FirstOrDefaultAsync(cancellationToken);

            if (friends is not null)
            {
                _logger.LogError("You cant accept a friend request from friend");
                throw new FriendRequestException("You cant accept a friend request from friend");
            }


            if (f!.ToUserId == userModel!.Id)
            {
                await _requestRepository.DeleteRequestAsync(f, cancellationToken);

                await _friendshipRepository.CreateFriendshipAsync(new Friendship()
                {
                    UserId = userModel!.Id,
                    FriendId = f!.Sender.Id,
                }, cancellationToken);
            }
            else
            {
                _logger.LogError("User is not receiver");
                throw new FriendRequestException("User is not receiver");
            }

        }
        else
        {
            throw new RequestException("Invalid request type.");
        }
        //TODO:Notification
        //// in notification box
        //return await _notificationRepository.CreateNotification(new FriendRequestNotification()
        //{
        //    NotificationMessage =
        //        $"Friend request to {friendRequest.ToUser.Profile.Name} {friendRequest.ToUser.Profile.Surname} was accepted",
        //    CreatedAt = DateTime.Now,
        //    IsRead = false,
        //    ToUserId = friendRequest!.Sender.Id,
        //    InitiatorId = friendRequest.ToUser.Id,
        //}, cancellationToken);


        //_logger.LogError("User is not receiver");
        //throw new FriendRequestException("User is not receiver");

    }

    public async Task CancelFriendRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var friendRequest = await _requestRepository.GetByIdAsync(requestId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(friendRequest,
            new FriendRequestException($"Friend request by id {requestId} not found"));

        if (friendRequest is FriendRequest fr)
        {
            if (fr!.ToUserId == userModel!.Id)
            {
                await _requestRepository.DeleteRequestAsync(fr, cancellationToken);
            }
            else
            {
                _logger.LogError("User is not receiver");
                throw new FriendRequestException("User is not receiver");
            }
        }
        else
        {
            throw new RequestException("Invalid request type.");
        }
    }

    public async Task<int> SendGroupRequestAsync(GroupRequestModel groupRequestModel, CancellationToken cancellationToken = default)
    {
        var senderModel = await _userService.GetByIdAsync(groupRequestModel.SenderId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(senderModel, new UserNotFoundException($"User with ID {groupRequestModel.SenderId} not found."));

        var receiverModel = await _groupRepository.GetByIdAsync(groupRequestModel.ToGroupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(receiverModel, new GroupNotFoundException($"Group with ID {groupRequestModel.ToGroupId} not found."));

        var userInGroup = await _groupRepository.IsUserInGroupAsync(senderModel!.Id, receiverModel!, cancellationToken);

        if (userInGroup is true)
        {
            _logger.LogError("You cant send a group request to group , you are already groupmember");
            throw new GroupRequestException("You cant send a group request to group , you are already groupmember");
        }

        if (!receiverModel!.IsPrivate)
        {
            throw new GroupRequestException("Group is not private and you can not to send request.");
        }

        var requestExists = await _requestRepository.RequestExists(new GroupRequest() { SenderId = senderModel!.Id, ToGroupId = receiverModel!.Id }, cancellationToken);
        if (requestExists is true)
        {
            _logger.LogError("Group request already exists");
            throw new GroupRequestException("Group request already exists");
        }

        var existingBan = await _bannedUserListRepository.GetAll()
       .Where(b => b.UserId == senderModel!.Id && b.GroupId == receiverModel.Id)
       .SingleOrDefaultAsync(cancellationToken);

        if (existingBan != null)
        {
            throw new BannedUserException($"User with ID {senderModel!.Id} is already banned in group {receiverModel.Id}");
        }

        return await _requestRepository.CreateRequestAsync(new GroupRequest()
        {
            SenderId = senderModel!.Id,
            ToGroupId = receiverModel!.Id,
            CreatedAt = DateTime.Now,
        }, cancellationToken);

    }

    public async Task AcceptGroupRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));

        var groupRequest = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupRequest, new GroupRequestException($"Group request by id {requestId} not found"));

        if (groupRequest is GroupRequest g)
        {
            var groupModel = await _groupRepository.GetByIdAsync(g.ToGroupId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(groupModel, new GroupNotFoundException($"Group by id {groupModel!.Id} not found"));

            var groupMemberdb = await _groupMemberRepository.GetByUserIdAndGroupIdAsync(userId, g.ToGroupId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(groupMemberdb, new UserNotFoundException($"User with this Id {userId} not found"));

            var userInGroup = await _groupRepository.IsUserInGroupAsync(g.SenderId, groupModel, cancellationToken);

            if (userInGroup is true)
            {
                _logger.LogError("You cant accept a group request from groupmember");
                throw new GroupRequestException("You cant accept a group request from groupmember");
            }

            var access = new List<GroupAccess>
            {
                GroupAccess.InviteToGroupMembers
            };
            var hasUserAccess = groupMemberdb!.RoleGroup.HasAccess(access);

            if (hasUserAccess || groupMemberdb.IsCreator)
            {
                await _requestRepository.DeleteRequestAsync(g, cancellationToken);
                await _groupService.JoinGroup(g!.ToGroupId, g!.SenderId, cancellationToken);
            }
            else { throw new NoRightException($"You have no rights for it"); }

        }
        else
        {
            throw new RequestException("Invalid request type.");
        }
        //TODO:Notification
        //// in notification box
        //return await _notificationRepository.CreateNotification(new FriendRequestNotification()
        //{
        //    NotificationMessage =
        //        $"Friend request to {friendRequest.ToUser.Profile.Name} {friendRequest.ToUser.Profile.Surname} was accepted",
        //    CreatedAt = DateTime.Now,
        //    IsRead = false,
        //    ToUserId = friendRequest!.Sender.Id,
        //    InitiatorId = friendRequest.ToUser.Id,
        //}, cancellationToken);



    }
    public async Task CancelGroupRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var groupRequest = await _requestRepository.GetByIdAsync(requestId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(groupRequest, new GroupRequestException($"Group request by id {requestId} not found"));

        if (groupRequest is GroupRequest g)
        {
            var groupMemberdb = await _groupMemberRepository.GetByUserIdAndGroupIdAsync(userId, g.ToGroupId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(groupMemberdb, new UserNotFoundException($"User with this Id {userId} not found"));

            var access = new List<GroupAccess>
            {
                GroupAccess.InviteToGroupMembers
            };
            var hasUserAccess = groupMemberdb!.RoleGroup.HasAccess(access);

            if (hasUserAccess || groupMemberdb.IsCreator)
                await _requestRepository.DeleteRequestAsync(g, cancellationToken);
            else { throw new NoRightException($"You have no rights for it"); }

        }
        else
        {
            throw new RequestException("Invalid request type.");
        }
    }

    public async Task<PaginationResultModel<BaseRequestModel>> GetAllSentFriendRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {

        var userRequests = await _requestRepository.GetAll()
            .Where(u => u is FriendRequest)
            .Where(u => u.SenderId == userId)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var userRequestsModels = _mapper.Map<IEnumerable<BaseRequestModel>>(userRequests);

        var paginationModel = new PaginationResultModel<BaseRequestModel>
        {
            Data = userRequestsModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = userRequests.Count,
        };

        return paginationModel;
    }
    public async Task<PaginationResultModel<BaseRequestModel>> GetAllSentGroupRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        var userRequests = await _requestRepository.GetAll()
            .Where(u => u is GroupRequest)
            .Where(u => u.SenderId == userId)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var userRequestsModels = _mapper.Map<IEnumerable<BaseRequestModel>>(userRequests);

        var paginationModel = new PaginationResultModel<BaseRequestModel>
        {
            Data = userRequestsModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = userRequests.Count,
        };

        return paginationModel;
    }
    public async Task<PaginationResultModel<BaseRequestModel>> GetAllIncomeFriendRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        var userRequests = await _requestRepository.GetAll()
            .Where(u =>u is FriendRequest && ((FriendRequest)u).ToUserId == userId)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var userRequestsModels = _mapper.Map<IEnumerable<BaseRequestModel>>(userRequests);

        var paginationModel = new PaginationResultModel<BaseRequestModel>
        {
            Data = userRequestsModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = userRequests.Count,
        };

        return paginationModel;
    }

    public async Task<PaginationResultModel<BaseRequestModel>> GetAllIncomeGroupRequest(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        var userRequests = await _requestRepository.GetAll()
            .Where(u => u is GroupRequest && ((GroupRequest)u).ToGroupId == userId)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var userRequestsModels = _mapper.Map<IEnumerable<BaseRequestModel>>(userRequests);

        var paginationModel = new PaginationResultModel<BaseRequestModel>
        {
            Data = userRequestsModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = userRequests.Count,
        };

        return paginationModel;
    }
}


