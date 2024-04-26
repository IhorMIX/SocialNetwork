using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FriendRequestController : ControllerBase
{

    private readonly ILogger<FriendRequestController> _logger;
    private readonly IMapper _mapper;
    private readonly IRequestService _requestService;

    public FriendRequestController(ILogger<FriendRequestController> logger, IMapper mapper, IRequestService requestService)
    {
        _logger = logger;
        _mapper = mapper;
        _requestService = requestService;
    }

    [HttpPost("{receiverId:int}")]
    public async Task<IActionResult> SendFriendRequest(int receiverId, CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        await _requestService.SendFriendRequestAsync(new FriendRequestModel() { SenderId = userId, ToUserId = receiverId }, cancellationToken);
        return Ok();
    }

    [HttpPost("accept/{requestId:int}")]
    public async Task<IActionResult> AcceptFriendRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.AcceptFriendRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpPost("cancel/{requestId:int}")]
    public async Task<IActionResult> CancelFriendRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.CancelFriendRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpGet("sent")]
    public async Task<IActionResult> GetAllSentFriendRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllSentFriendRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }

    [HttpGet("received")]
    public async Task<IActionResult> GetAllIncomeFriendRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllIncomeFriendRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }
}