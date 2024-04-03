using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class MessageServiceTest : BaseMessageTestService<IMessageService, MessageService>
{
    
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<IMessageReadStatusRepository, MessageReadStatusRepository>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        
        base.SetUpAdditionalDependencies(services);
    }
    
    [Test]
    public async Task CreateMessage_MessageCreated_returnLastMessage()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        
        var chatService = ServiceProvider.GetRequiredService<IChatService>();
        await chatService.CreateP2PChat(user1.Id, user2.Id, new ChatModel
        {
            Name = "Chat1",
            Logo = "null",
            IsGroup = true,
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };
        var chatList = await chatService.FindChatByName(user1.Id,paginationModel, "Chat1");
        var chat = chatList.Data.First();

        var messageService = ServiceProvider.GetRequiredService<IMessageService>();
        var createdMessage = await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test.png",
                }
            }
        });

        Assert.That(createdMessage, Is.Not.EqualTo(null));
        Assert.That(createdMessage.Text, Is.EqualTo("Test message"));
        Assert.That(createdMessage.Files!.Any(f => f.FilePath == "test.png"));

    }


    [Test]
    public async Task ReadMessage_GetMessages_returnMessages()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);
        await friendService.AddFriendshipAsync(user1!.Id, user3!.Id);
        
        var chatService = ServiceProvider.GetRequiredService<IChatService>();
        await chatService.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            IsGroup = true,
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };
        var chatList = await chatService.FindChatByName(user1.Id,paginationModel, "Chat2");
        var chat = chatList.Data.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });

        var messageService = ServiceProvider.GetRequiredService<IMessageService>();
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test1.png",
                }
            }
        });
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test2.png",
                }
            }
        });
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test3.png",
                }
            }
        });

        var messages = await Service.GetMessagesAsync(user2.Id, chat.Id);

        Assert.That(messages.Count() == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3"));
    }

    [Test]
    public async Task ReplyMessage_EditReplyMessage_DeleteMessageToReply_ReturnMessages()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);
        await friendService.AddFriendshipAsync(user1!.Id, user3!.Id);
        
        var chatService = ServiceProvider.GetRequiredService<IChatService>();
        await chatService.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            IsGroup = true,
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };
        var chatList = await chatService.FindChatByName(user1.Id,paginationModel, "Chat2");
        var chat = chatList.Data.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });
        
        await Service.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test1.png",
                }
            }
        });
        var messageToReply = await Service.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test2.png",
                }
            }
        });
        var replyMessage = await Service.ReplyMessageAsync(user3.Id, chat.Id, messageToReply.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test3.png",
                }
            }
        });

        var messages = await Service.GetMessagesAsync(user2.Id, chat.Id);

        Assert.That(messages.Count() == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3"));
        Assert.That(replyMessage.ChatId == chat.Id && replyMessage.ToReplyMessageId == messageToReply.Id);

        await Service.EditMessageAsync(user3.Id, chat.Id, replyMessage.Id, new MessageModel
        {
            Text = "editedMessage",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test3.png",
                },
                new FileInMessageModel
                {
                    FilePath = "test4.png",
                }
            }
        });
        
        messages = await Service.GetMessagesAsync(user2.Id, chat.Id);
        replyMessage = messages.FirstOrDefault(c => c.Id == replyMessage.Id);

        Assert.That(messages.Any(c => c.Text == "editedMessage" && c.Text == "editedMessage"));
        Assert.That(replyMessage!.ChatId == chat.Id && replyMessage.Text == "editedMessage" && replyMessage.ToReplyMessageId == messageToReply.Id);
        
        await Service.DeleteMessageAsync(user1.Id, chat.Id, messageToReply.Id, false);
        messages = await Service.GetMessagesAsync(user2.Id, chat.Id);
        replyMessage = messages.FirstOrDefault(c => c.Id == replyMessage.Id);
        Assert.That(replyMessage!.ChatId == chat.Id && replyMessage.ToReplyMessageId == null);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));

        var lastMsg = await Service.GetLastMessageAsync(user1.Id, chat.Id);
        Assert.That(lastMsg.Text == "editedMessage");
    }
    
    [Test]
    public async Task CreateMessage_DeleteForAuthor_GetOnlyUndeleted()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);
        await friendService.AddFriendshipAsync(user1!.Id, user3!.Id);

        var chatService = ServiceProvider.GetRequiredService<IChatService>();
        await chatService.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            IsGroup = true,
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };
        var chatList = await chatService.FindChatByName(user1.Id,paginationModel, "Chat2");
        var chat = chatList.Data.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });
        
        await Service.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test1.png",
                }
            }
        });
        var messageToReply = await Service.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test2.png",
                }
            }
        });
        var replyMessage = await Service.ReplyMessageAsync(user3.Id, chat.Id, messageToReply.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = new List<FileInMessageModel>
            {
                new FileInMessageModel
                {
                    FilePath = "test3.png",
                }
            }
        });
        
        await Service.DeleteMessageAsync(user3.Id, chat.Id, replyMessage.Id, true);
        var messages = await Service.GetMessagesAsync(user3.Id, chat.Id);
        Assert.That(messages.Count == 2);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3") is false);
        
        messages = await Service.GetMessagesAsync(user2.Id, chat.Id);
        Assert.That(messages.Count == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3"));
        
        
        Assert.That(replyMessage.ChatId == chat.Id && replyMessage.ToReplyMessageId == messageToReply.Id);
    }
    
}