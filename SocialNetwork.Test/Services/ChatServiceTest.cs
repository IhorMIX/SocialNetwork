using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class ChatServiceTest : DefaultServiceTest<IChatService, ChatService>
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
        
        
        base.SetUpAdditionalDependencies(services);
    }
    private async Task CreateRole()
    {
        var _roleRepository =  ServiceProvider.GetRequiredService<IRoleRepository>();
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "@everyone",
            RoleColor = "null",
            SendMessages = true,
            SendAudioMess = true,
            SendFiles = true,
            EditRoles = false,
            AddMembers = false,
            DelMembers = false,
            MuteMembers = false,
            DelMessages = false,
            EditNicknames = false
        });
        
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "Admin",
            RoleColor = "null",
            SendMessages = true,
            SendAudioMess = true,
            SendFiles = true,
            EditRoles = true,
            AddMembers = true,
            DelMembers = true,
            MuteMembers = true,
            DelMessages = true,
            EditNicknames = true
        });
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "P2PAdmin",
            RoleColor = "null",
            SendMessages = true,
            SendAudioMess = true,
            SendFiles = true,
            EditRoles = false,
            AddMembers = false,
            DelMembers = false,
            MuteMembers = false,
            DelMessages = true,
            EditNicknames = false
        });
    }
    
    [Test]
    public async Task CreateP2PChat_Ok_ChatCreated()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        await userService.CreateUserAsync(user2);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);

        await CreateRole();

        await Service.CreateP2PChat(user1.Id, user2.Id, new ChatModel
        {
            Name = "Chat1",
            Logo = "null",
            isGroup = true,
        });

        var chatList = await Service.FindChatByName(user1.Id, "Chat1");
        var chat = chatList.First();
        foreach (var chatMember in chat.ChatMembers)
        {
            Assert.That(chatMember.Role.Count == 1);
        }
    }
    
    [Test]
    public async Task CreateGroupChat_AddMember_ChatCreatedWith2Members()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var user3 = await UserModelHelper.CreateTestData();
        var user4 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        await userService.CreateUserAsync(user2);
        user2 = await userService.GetUserByLogin(user2.Login);
        await userService.CreateUserAsync(user3);
        user3 = await userService.GetUserByLogin(user3.Login);
        await userService.CreateUserAsync(user4);
        user4 = await userService.GetUserByLogin(user4.Login);
        
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        Assert.That(user4, Is.Not.EqualTo(null));
        
        await CreateRole();
        
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            isGroup = true,
        });
       
        var chat = await Service.FindChatByName(user1.Id, "Chat2");
        Assert.That(chat.Count == 1);
        
        await Service.AddUsers(user1.Id, chat.First().Id, new List<int>{user2!.Id, user3!.Id, user4.Id});
        chat = await Service.FindChatByName(user1.Id, "Chat2");
        Assert.That(chat.First().ChatMembers!.Count == 4);
        
        await Service.DelMember(user1.Id, chat.First().Id, user2.Id);
        chat = await Service.FindChatByName(user1.Id, "Chat2");
        Assert.That(chat.First().ChatMembers!.Count == 3);
    }
    
    [Test]
    public async Task CreateGroupChats_GetChats_ChatCreatedWith2Members()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        Assert.That(user1, Is.Not.EqualTo(null));

        await CreateRole();
        
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chats1",
            Logo = "null",
            isGroup = true,
        });
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chats2",
            Logo = "null",
            isGroup = true,
        });
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chats3",
            Logo = "null",
            isGroup = true,
        });
       
        var chat = await Service.GetAllChats(user1.Id);
        Assert.That(chat.Count == 3);
    }
    
    

    [Test]
    public async Task CreateRole_EditRole_RoleEdited()
    {
        
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var user3 = await UserModelHelper.CreateTestData();
        var user4 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        await userService.CreateUserAsync(user2);
        user2 = await userService.GetUserByLogin(user2.Login);
        await userService.CreateUserAsync(user3);
        user3 = await userService.GetUserByLogin(user3.Login);
        await userService.CreateUserAsync(user4);
        user4 = await userService.GetUserByLogin(user4.Login);
        
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        Assert.That(user4, Is.Not.EqualTo(null));
        
        await CreateRole();
        
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat3",
            Logo = "null",
            isGroup = true,
        });
        
        var chat = (await Service.FindChatByName(user1.Id, "Chat3")).First();
        Assert.That(chat is not null);
        
        await Service.AddUsers(user1.Id, chat.Id, new List<int>{user2!.Id, user3!.Id});
        
        chat = (await Service.FindChatByName(user1.Id, "Chat3")).First();
        Assert.That(chat.ChatMembers!.Count == 3);
        
        await Service.AddRole(user1.Id, chat.Id,new RoleModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });
        
        var role = (await Service.GetAllChatRoles(user1.Id, chat.Id)).First();
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, chat.Id, role.Id, new List<int>(){user2.Id, user3.Id});
        
        role = (await Service.GetAllChatRoles(user1.Id, chat.Id)).First();
        chat = (await Service.FindChatByName(user1.Id, "Chat3")).First();
        Assert.That(chat.ChatMembers.Any(m => m.Role.Any(r => r.RoleName == role.RoleName) && m.User.Login == user2.Login));

        role.RoleName = "Role21";
        role.RoleColor = "black1";
        role.DelMessages = true;
        role.EditNicknames = true;
        await Service.EditRole(user1.Id, chat.Id, role.Id, role);
        
        role = (await Service.GetAllChatRoles(user1.Id, chat.Id)).First();
        chat = (await Service.FindChatByName(user1.Id, "Chat3")).First();
        Assert.That(role.RoleName != "Role2" &&
                    chat.ChatMembers.Any(c => c.Role.Any(r => r.RoleName != "Role2")));
        Assert.That(role.DelMessages != false && role.EditNicknames == true);
    }
    
    [Test]
    public async Task CreateRole_DelRole_RoleDeleted()
    {
        
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var user3 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        await userService.CreateUserAsync(user2);
        user2 = await userService.GetUserByLogin(user2.Login);
        await userService.CreateUserAsync(user3);
        user3 = await userService.GetUserByLogin(user3.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        await CreateRole();
        
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat4",
            Logo = "null",
            isGroup = true,
        });
        
        var chat = (await Service.FindChatByName(user1.Id, "Chat4")).First();
        
        Assert.That(chat is not null);
        
        await Service.AddUsers(user1.Id, chat.Id, new List<int>{user2!.Id, user3!.Id});
        chat = (await Service.FindChatByName(user1.Id, "Chat4")).First();
        Assert.That(chat.ChatMembers!.Count == 3);
        
        await Service.AddRole(user1.Id, chat.Id,new RoleModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });
        
        var role = (await Service.GetAllChatRoles(user1.Id, chat.Id)).First();
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, chat.Id, role.Id, new List<int>(){user2.Id, user3.Id});
        chat = (await Service.FindChatByName(user1.Id, "Chat4")).First();
        Assert.That(chat.ChatMembers
            .Any(m => m.Role
                .Any(r => r.RoleName == role.RoleName) && 
                      m.User.Id == user2.Id));
        
        Assert.That(chat.ChatMembers
            .Any(m => m.Role
                          .Any(r => r.RoleName == role.RoleName) && 
                      m.User.Id == user3.Id));
        
        await Service.UnSetRole(user1.Id, chat.Id, role.Id, new List<int>(){user2.Id, user3.Id});
        chat = (await Service.FindChatByName(user1.Id, "Chat4")).First();
        
        Assert.That(chat.ChatMembers
            .Any(m => m.Role.Any(r => r.RoleName == role.RoleName&& 
                                      m.User.Id == user2.Id)) == false);
        
        Assert.That(chat.ChatMembers
            .Any(m => m.Role.Any(r => r.RoleName == role.RoleName&& 
                                      m.User.Id == user3.Id)) == false);
        
        await Service.DelRole(user1.Id, chat.Id, role.Id);
        Assert.That((await Service.GetAllChatRoles(user1.Id, chat.Id)).Count == 0);
    }
}