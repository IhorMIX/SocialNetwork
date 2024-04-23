using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    
    private readonly ILogger<NotificationController> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public NotificationController(ILogger<NotificationController> logger, IMapper mapper, INotificationService notificationService)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notification = await _notificationService.GetByUserId(userId, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<BaseNotificationViewModel>>(notification));
    }
    
    [HttpGet("box-notifications")]
    public async Task<IActionResult> GetInBoxNotifications( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notification = await _notificationService.GetBoxNotificationsByUserId(userId, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<BaseNotificationViewModel>>(notification));
    }
    
    [HttpPut("{notificationId:int}")]
    public async Task<IActionResult> ReadNotifications(int notificationId,  CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _notificationService.ReadNotification(userId, notificationId, cancellationToken);
        return Ok();
    }

    [HttpDelete("{notificationId:int}")]
    public async Task<IActionResult> RemoveNotification(int notificationId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _notificationService.RemoveNotification(userId, notificationId, cancellationToken);
        return Ok();
    }
}