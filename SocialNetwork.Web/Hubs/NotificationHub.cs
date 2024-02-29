using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;
using System.Text.Json;
using AutoMapper;
using SocialNetwork.BLL.Services.Interfaces;

namespace SocialNetwork.Web.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public NotificationHub(INotificationService notificationService, IMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    { 
        var userId = Context.GetHttpContext()!.User.GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        await base.OnConnectedAsync();
    } 
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        await base.OnDisconnectedAsync(exception);
    }
}