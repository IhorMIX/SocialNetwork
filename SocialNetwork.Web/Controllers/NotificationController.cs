using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Extensions;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    
    private readonly ILogger<FriendshipController> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public NotificationController(ILogger<FriendshipController> logger, IMapper mapper, INotificationService notificationService)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notification = await _notificationService.GetByUserId(userId, cancellationToken);
        return Ok(notification);
    }
    
    [HttpPut("notification")]
    public async Task<IActionResult> ReadNotifications([FromQuery] int notificationId,  CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _notificationService.ReadNotification(userId, notificationId, cancellationToken);
        return Ok();
    }

    [HttpDelete("notification")]
    public async Task<IActionResult> RemoveNotification([FromQuery] int notificationId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _notificationService.RemoveNotification(userId, notificationId, cancellationToken);
        return Ok();
    }
}