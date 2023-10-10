using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class MessageServiceTest : DefaultServiceTest<IMessageService, MessageService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
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
            isGroup = true,
        });

        var chatList = await chatService.FindChatByName(user1.Id, "Chat1");
        var chat = chatList.First();

        var messageService = ServiceProvider.GetRequiredService<IMessageService>();
        var createdMessage = await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message",
            Files = "test.png"
        });

        Assert.That(createdMessage, Is.Not.EqualTo(null));
        Assert.That(createdMessage.Text, Is.EqualTo("Test message"));
        Assert.That(createdMessage.Files, Is.EqualTo("test.png"));

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
            isGroup = true,
        });

        var chatList = await chatService.FindChatByName(user1.Id, "Chat2");
        var chat = chatList.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });

        var messageService = ServiceProvider.GetRequiredService<IMessageService>();
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = "test1.png"
        });
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = "test2.png"
        });
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = "test3.png"
        });

        var messages = await Service.GetMessages(user2.Id, chat.Id);

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
            isGroup = true,
        });

        var chatList = await chatService.FindChatByName(user1.Id, "Chat2");
        var chat = chatList.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });
        
        await Service.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = "test1.png"
        });
        var messageToReply = await Service.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = "test2.png"
        });
        var replyMessage = await Service.ReplyMessage(user3.Id, chat.Id, messageToReply.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = "test3.png"
        });

        var messages = await Service.GetMessages(user2.Id, chat.Id);

        Assert.That(messages.Count() == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3"));
        Assert.That(replyMessage.ChatId == chat.Id && replyMessage.ToReplyMessageId == messageToReply.Id);

        await Service.EditMessage(user3.Id, chat.Id, replyMessage.Id, new MessageModel
        {
            Text = "editedMessage",
        });
        
        messages = await Service.GetMessages(user2.Id, chat.Id);
        replyMessage = messages.FirstOrDefault(c => c.Id == replyMessage.Id);

        Assert.That(messages.Any(c => c.Text == "editedMessage" && c.Files == "test3.png"));
        Assert.That(replyMessage.ChatId == chat.Id && replyMessage.Text == "editedMessage" && replyMessage.ToReplyMessageId == messageToReply.Id);
        
        await Service.DeleteMessage(user1.Id, chat.Id, messageToReply.Id, false);
        messages = await Service.GetMessages(user2.Id, chat.Id);
        replyMessage = messages.FirstOrDefault(c => c.Id == replyMessage.Id);
        Assert.That(replyMessage!.ChatId == chat.Id && replyMessage.ToReplyMessageId == null);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));

        var lastMsg = await Service.GetLastMessage(user1.Id, chat.Id);
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
            isGroup = true,
        });

        var chatList = await chatService.FindChatByName(user1.Id, "Chat2");
        var chat = chatList.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });
        
        await Service.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = "test1.png"
        });
        var messageToReply = await Service.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = "test2.png"
        });
        var replyMessage = await Service.ReplyMessage(user3.Id, chat.Id, messageToReply.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = "test3.png"
        });
        
        await Service.DeleteMessage(user3.Id, chat.Id, replyMessage.Id, true);
        var messages = await Service.GetMessages(user3.Id, chat.Id);
        Assert.That(messages.Count == 2);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3") is false);
        
        messages = await Service.GetMessages(user2.Id, chat.Id);
        Assert.That(messages.Count == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 1"));
        Assert.That(messages.Any(c => c.Text == "Test message 2"));
        Assert.That(messages.Any(c => c.Text == "Test message 3"));
        
        
        Assert.That(replyMessage.ChatId == chat.Id && replyMessage.ToReplyMessageId == messageToReply.Id);
    }
    
}