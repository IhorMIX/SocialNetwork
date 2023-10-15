using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;

    public ChatHub(IMessageService messageService, IMapper mapper, IUserService userService, IChatService chatService)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _chatService = chatService;
    }
    
    public async Task SendMessage(int chatId, string textMess, List<FileModel> files)
    {
        
        var userId = Context.GetHttpContext()!.User.GetUserId();
        
        var message = await _messageService.CreateMessage(userId, chatId,
            new MessageModel()
            {
                Text = textMess, 
                FileModels = files,
            }, CancellationToken.None);
        
        await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
        
        await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId).SendAsync("UserTyping", userId);
    }

    public async Task ReplyMessage(int chatId, string textMess, List<FileModel> files, int messageToReplyId)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        
        var message = await _messageService.ReplyMessage(userId, chatId, messageToReplyId,
            new MessageModel()
            {
                Text = textMess, 
                FileModels = files,
            }, CancellationToken.None);
        
        await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
        
        await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId).SendAsync("UserTyping", userId);
    }
    
    public async Task TextTyping(int chatId)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var user = await _userService.GetByIdAsync(userId, CancellationToken.None);
        
        await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId).SendAsync("TextTyping", user!.Profile.Name);
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        await _userService.ChangeOnlineStatus(userId, CancellationToken.None);
        var userChats = await _chatService.GetAllChats(userId, CancellationToken.None);
        
        foreach (var chat in userChats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
        }
        await base.OnConnectedAsync();
    } 
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        await _userService.ChangeOnlineStatus(userId, CancellationToken.None);
        var userChats = await _chatService.GetAllChats(userId, CancellationToken.None);
        foreach (var chat in userChats)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
        }
        await base.OnConnectedAsync();
    }
}