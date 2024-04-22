using System.Text.Json;
using AutoMapper;
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
public class CommentPostController : ControllerBase
{
    private readonly ILogger<CommentPostController> _logger;
    private readonly IMapper _mapper;
    private readonly ICommentPostService _commentPostService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    
    public CommentPostController(ILogger<CommentPostController> logger, IMapper mapper, ICommentPostService commentPostService, IHubContext<NotificationHub> notificationHubContext, INotificationService notificationService)
    {
        _logger = logger;
        _mapper = mapper;
        _commentPostService = commentPostService;
        _notificationHubContext = notificationHubContext;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] AddCommentModel commentModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notificationId = await _commentPostService.CommentPostAsync(userId, commentModel.PostId, commentModel.Text, cancellationToken);
        
        if (notificationId is not null)
        {
            var notificationModel = await _notificationService.GetByIdAsync(notificationId.Value, cancellationToken);
            await _notificationHubContext.Clients.Group(notificationModel!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notificationModel)), cancellationToken: cancellationToken);
        }
        
        return Ok();
    }
    
    [HttpPost("reply-on")]
    public async Task<IActionResult> ReplyOnComment([FromBody] ReplyOnCommentModel commentModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notificationId = await _commentPostService.ReplyOnCommentAsync(userId, commentModel.CommentId, commentModel.Text, cancellationToken);
        
        if (notificationId is not null)
        {
            var notificationModel = await _notificationService.GetByIdAsync(notificationId.Value, cancellationToken);
            await _notificationHubContext.Clients.Group(notificationModel!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notificationModel)), cancellationToken: cancellationToken);
        }
        
        return Ok();
    }
    
    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> RemoveComment(int commentId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _commentPostService.RemoveCommentAsync(userId, commentId, cancellationToken);
        return Ok();
    }
    
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetComments(int postId, [FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    { 
        var userId = User.GetUserId();
        var comments = await _commentPostService.GetCommentsAsync(userId, postId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<CommentPostViewModel>>(comments));
    }
    
}