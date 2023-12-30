using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;


namespace SocialNetwork.Web.Hubs;

[Authorize]
public class OnlineStatusHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly DelayedWriter _delayedWriter;

    public OnlineStatusHub(IMapper mapper, IUserService userService, DelayedWriter delayedWriter)
    {
        _mapper = mapper;
        _userService = userService;
        _delayedWriter = delayedWriter;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        await _userService.ChangeOnlineStatus(userId, CancellationToken.None);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();

        Func<CancellationToken, Task> action = async (cancellationToken) =>
        {
            await _userService.ChangeOnlineStatus(userId, cancellationToken);
        };
        
        await _delayedWriter.QueueUserStatusChangeAsync(action, CancellationToken.None);
        await base.OnDisconnectedAsync(exception);
    }
}