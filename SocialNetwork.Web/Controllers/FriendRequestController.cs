using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FriendRequestController : ControllerBase
{
    private readonly ILogger<FriendshipController> _logger;
    private readonly IMapper _mapper;
    private readonly IFriendRequestService _friendRequestService;

    public FriendRequestController(ILogger<FriendshipController> logger, IMapper mapper, IFriendRequestService friendRequestService)
    {
        _logger = logger;
        _mapper = mapper;
        _friendRequestService = friendRequestService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendRequest([FromQuery]int receiverId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendRequestService.SendRequest(userId, receiverId, cancellationToken);
        return Ok();
    }
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptRequest([FromQuery]int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendRequestService.AcceptRequest(userId, requestId, cancellationToken);
        return Ok();
    }
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelRequest([FromQuery]int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendRequestService.CancelRequest(userId, requestId, cancellationToken);
        return Ok();
    }
    [HttpGet("received")]
    public async Task<IActionResult> GetAllIncomeRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _friendRequestService.GetAllIncomeRequest(userId,pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<FriendRequestViewModel>>(requests));
    }
    [HttpGet("sent")]
    public async Task<IActionResult> GetAllSentRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _friendRequestService.GetAllSentRequest(userId,pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<FriendRequestViewModel>>(requests));
    }
}