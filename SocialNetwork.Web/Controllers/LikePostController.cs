using AutoMapper;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Hubs;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikePostController : ControllerBase
{
    private readonly ILogger<LikePostController> _logger;
    private readonly IMapper _mapper;
    private readonly ILikePostService _likePostService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    public LikePostController(ILogger<LikePostController> logger, IMapper mapper, ILikePostService likePostService, INotificationService notificationService, IHubContext<NotificationHub> notificationHubContext)
    {
        _logger = logger;
        _mapper = mapper;
        _likePostService = likePostService;
        _notificationService = notificationService;
        _notificationHubContext = notificationHubContext;
    }

    [HttpPost("{postId:int}")]
    public async Task<IActionResult> LikePost(int postId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notificationId = await _likePostService.LikePostAsync(userId, postId, cancellationToken);
        if (notificationId is not null)
        {
            var notificationModel = await _notificationService.GetByIdAsync(notificationId.Value, cancellationToken);
            await _notificationHubContext.Clients.Group(notificationModel!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notificationModel)), cancellationToken: cancellationToken);
        }
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetLikes([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var likes = await _likePostService.GetUsersLikesAsync(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<LikePostViewModel>>(likes));
    }
    
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetPostLikes(int postId, [FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var likes = await _likePostService.GetPostLikesAsync(userId, postId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<LikePostViewModel>>(likes));
    }
    
}