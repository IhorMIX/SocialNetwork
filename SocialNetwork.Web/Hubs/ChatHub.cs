using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IReactionService _reactionService;
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IUserInChatTracker _userTracker;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    
    public ChatHub(IMessageService messageService, IMapper mapper, IUserService userService, IChatService chatService,
        IReactionService reactionService, INotificationService notificationService, IUserInChatTracker userTracker, IHubContext<NotificationHub> notificationHubContext)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _chatService = chatService;
        _reactionService = reactionService;
        _notificationService = notificationService;
        _userTracker = userTracker;
        _notificationHubContext = notificationHubContext;
    }

    public async Task SendMessage(int chatId, string textMess, List<FileSend> files)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var connectedUsers = (_userTracker.GetUsersInGroup(chatId.ToString())).ConvertAll(int.Parse);
        
        var messageModel = await _messageService.CreateMessage(userId, chatId,
            new MessageModel()
            {
                Text = textMess,
                FileModels = _mapper.Map<List<FileModel>>(files),
            },
            CancellationToken.None);
        
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage",
            JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(messageModel)));
       
        var notifications = await _messageService.CreateNotification(messageModel, connectedUsers);
        
        foreach (var notification in notifications)
        {
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)));
        }
        await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId).SendAsync("UserTyping", userId);
    }

    public async Task ReplyMessage(int chatId, string textMess, List<FileSend> files, int messageToReplyId)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var connectedUsers = (_userTracker.GetUsersInGroup(chatId.ToString())).ConvertAll(int.Parse);
        
        var messageModel = await _messageService.ReplyMessageAsync(userId, chatId, messageToReplyId,
            new MessageModel()
            {
                Text = textMess,
                FileModels = _mapper.Map<List<FileModel>>(files),
            },
            CancellationToken.None);
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveReplyOnMessage",
            JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(messageModel)));

        var notifications = await _messageService.CreateNotification(messageModel, connectedUsers);
        foreach (var notification in notifications)
        {
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)));
        }
        
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
        var chatIdValues = Context.GetHttpContext()!.Request.Query["chatId"];
        
        if (!string.IsNullOrEmpty(chatIdValues))
        {
            var stringChatId = chatIdValues[0];
            var userId = Context.GetHttpContext()!.User.GetUserId();
            var isInChat =  await _chatService.UserInChatCheck(userId, int.Parse(stringChatId!));
            
            if (isInChat)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, stringChatId!);
                _userTracker.AddToGroup(userId.ToString(), stringChatId!);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var chatIdValues = Context.GetHttpContext()!.Request.Query["chatId"];
        if (!string.IsNullOrEmpty(chatIdValues))
        {
            var chatId = chatIdValues[0];
            var userId = Context.GetHttpContext()!.User.GetUserId();
            var userChats = (await _chatService.GetAllChats(userId, CancellationToken.None)).Data;
            
            if (userChats.Any(chat => chat.Id.ToString() == chatId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId!);
                _userTracker.RemoveFromGroup(userId.ToString(), chatId!);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }


    public async Task OpenChat(int chatId, PaginationModel paginationModel)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        
        var messages = await _messageService.GetMessagesAsync(userId, chatId, paginationModel, CancellationToken.None);
        await _messageService.ReadMessages(userId, chatId, messages.Data, CancellationToken.None);
        messages = await _messageService.GetMessagesAsync(userId, chatId, paginationModel, CancellationToken.None);

        await Clients.Caller.SendAsync("GetMessages",
            JsonSerializer.Serialize(_mapper.Map<PaginationResultViewModel<MessageViewModel>>(messages)));
    }
    
    public async Task AddReaction(int chatId, int messageId, AddReactionModel reaction)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
      
        var reactionModel = await _reactionService.AddReaction(userId, messageId, _mapper.Map<ReactionModel>(reaction),
            CancellationToken.None);
        
        await Clients.Group(chatId.ToString()).SendAsync("AddReactionToMessage",
            JsonSerializer.Serialize(_mapper.Map<ReactionViewModel>(reactionModel)));
       
        if (reaction.Type != reactionModel.Type)
        {
            var notification = await _reactionService.CreateNotification(reactionModel);
        
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)));
        }
    }

    public async Task DelReaction(int chatId, int messageId, int reactionId)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var message = await _messageService.GetByIdAsync(messageId, CancellationToken.None);
        await Clients.Group(chatId.ToString()).SendAsync("RemoveReactionToMessage",
            JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
        await _reactionService.RemoveReaction(userId, chatId, reactionId, CancellationToken.None);
    }

    public async Task DelMessage(int chatId, int messageId, bool isForAuthor)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var message = await _messageService.GetByIdAsync(userId, chatId, messageId, CancellationToken.None);
        if (isForAuthor)
            await Clients.Caller.SendAsync("DelMessageForAuthor",
                JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
        else
            await Clients.Group(chatId.ToString()).SendAsync("DelMessage",
                JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
        await _messageService.DeleteMessageAsync(userId, chatId, messageId, isForAuthor, CancellationToken.None);
    }

    public async Task EditMessage(int chatId, int messageId, string textMess, List<FileSend> files)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var message = await _messageService.EditMessageAsync(userId, chatId, messageId,
            new MessageModel
            {
                Text = textMess,
                FileModels = _mapper.Map<List<FileModel>>(files),
            }, CancellationToken.None);
        await Clients.Group(chatId.ToString()).SendAsync("EditMessage", JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(message)));
    }

    public async Task GetMessagesByText(int chatId, string text)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var messages = await _messageService.GetMessagesByTextAsync(userId, chatId, text, CancellationToken.None);
        await Clients.Caller.SendAsync("GetMessagesByText", JsonSerializer.Serialize(_mapper.Map<List<MessageViewModel>>(messages)));
    }

    public async Task ShareWithMessage(int chatId, int messageId, bool showCreator)
    {
        var userId = Context.GetHttpContext()!.User.GetUserId();
        var connectedUsers = (_userTracker.GetUsersInGroup(chatId.ToString())).ConvertAll(int.Parse);
        
        var messageModel = await _messageService.ShareWithMessage(userId, messageId, chatId, showCreator);
        
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveReplyOnMessage",
            JsonSerializer.Serialize(_mapper.Map<MessageViewModel>(messageModel)));

        var notifications = await _messageService.CreateNotification(messageModel, connectedUsers);
        foreach (var notification in notifications)
        {
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)));
        }
        
        await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId).SendAsync("UserTyping", userId);
    }
    
}