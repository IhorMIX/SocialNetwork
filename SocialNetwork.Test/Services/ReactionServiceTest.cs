using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class ReactionServiceTest : BaseMessageTestService<IReactionService, ReactionService>
{
    
    [Test]
    public async Task CreateMessages_AddReactions_EditReaction()
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
        var message2 = await messageService.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = "test2.png"
        });
        var message3 = await messageService.ReplyMessage(user3.Id, chat.Id, message2.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = "test3.png"
        });
        
        await Service.AddReaction(user1.Id, message2.Id, new ReactionModel
        {
            Type = "smile"
        });
        await Service.AddReaction(user1.Id, message2.Id, new ReactionModel
        {
            Type = "like"
        });
        await Service.AddReaction(user1.Id, message3.Id, new ReactionModel
        {
            Type = "smile"
        });
        
        var messages = await messageService.GetMessages(user2.Id, chat.Id);

        Assert.That(messages.Count() == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 2" &&  c.Reactions.Any(c => c.Type == "like")));
        Assert.That(messages.Any(c => c.Text == "Test message 2" &&  c.Reactions.Any(c => c.Type == "smile")) is false);
        Assert.That(messages.Any(c => c.Text == "Test message 3" &&  c.Reactions.Any(c => c.Type == "smile")));

    }
    
}