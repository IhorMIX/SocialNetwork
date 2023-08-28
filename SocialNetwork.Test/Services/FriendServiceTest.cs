using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.BL.Services;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;
using System.Linq;

public class FriendServiceTest : DefaultServiceTest<IFriendshipService, FriendshipService>
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
    
    [Test]
    public async Task CreateFriendship_UsersFound_createdFriendship()
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
        
        await Service.AddFriendshipAsync(user1!.Id, user2!.Id);
        Assert.That(Service.GetByIdAsync(1), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task DeleteFriend_FriendFound_DeletedFriend()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        await Service.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByIdAsync(1), Is.Not.EqualTo(null));
    }
    [Test]
    public async Task DeleteFriend_FriendNotFound_DeletedFriend()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        await Service.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        Assert.ThrowsAsync<FriendNotFoundException>(async () => await Service.GetByIdAsync(1));
    }

    [Test]
    public async Task GetAllFriends_UserFound_ReturnFriends()
    {
        var user = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user);
        
        var user1 = await UserModelHelper.CreateTestData();
        user1.Login = "User231";
        user1.Profile.Email = "USER232@gmail.com";
        await userService.CreateUserAsync(user1);
        
        user1.Login = "User321";
        user1.Profile.Email = "USER321@gmail.com";
        await userService.CreateUserAsync(user1);

        user1 = await userService.GetUserByLogin(user.Login);
        var user2 = await userService.GetUserByLogin("User231");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);
        
        user1 = await userService.GetUserByLogin(user.Login);
        user2 = await userService.GetUserByLogin("User321");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);
        
        Assert.That(
            Service.GetAllFriends(user1.Id), 
            Is.Not.EqualTo(null));
        
        var friendList =await Service.GetAllFriends(user1.Id);
        Assert.That(friendList.Count() == 2);
    }
    
    [Test]
    public async Task GetFriendByEmail_UserFound_ReturnFriend()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var user3 = await UserModelHelper.CreateTestData();
        
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        await userService.CreateUserAsync(user3);
        
        var userDb1 = await userService.GetUserByLogin(user1.Login);
        var userDb2 = await userService.GetUserByLogin(user2.Login);
        await Service.AddFriendshipAsync(userDb1!.Id,userDb2!.Id);

        userDb1 = await userService.GetUserByLogin(user3.Login);
        var userDb3 = await userService.GetUserByLogin(user1.Login);
        await Service.AddFriendshipAsync(userDb3!.Id,userDb1!.Id);
        
        Assert.That(
            Service.FindFriendByEmail(userDb3.Id, userDb1.Profile.Email), 
            Is.Not.EqualTo(null));
        
        var Friend = await Service.FindFriendByEmail(
            userDb3.Id, userDb1.Profile.Email);
        
        var friendUser = await userService.GetUserByLogin(user3.Login);
        
        Assert.That(Friend.Profile.Email == friendUser?.Profile.Email);
        Assert.That(Friend.Login == friendUser?.Login);
    }
    
    [Test]
    public async Task GetAllFriendsByNameSurname_UserFound_ReturnFriends()
    {
        var user = await UserModelHelper.CreateTestData();
        var user1 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user);

        for (int i = 2; i < 12; i++)
        {
            user1.Login = "UserNameSurname" + i;
            user1.Profile.Email = i + "USER@gmail.com";
            user1.Profile.Surname = "Surname" + i;
            user1.Profile.Name = "Name" + i;
            await userService.CreateUserAsync(user1);
        }

        for (int i = 2; i < 12; i++)
        {
            var tempuser = await userService.GetUserByLogin(user.Login);
            var tempuser2 = await userService.GetUserByLogin("UserNameSurname" + i);
            await Service.AddFriendshipAsync(tempuser!.Id,tempuser2!.Id);
        }
        
        var userDb = await userService.GetUserByLogin(user.Login);
        Assert.That(
            Service.GetAllFriends(userDb.Id), 
            Is.Not.EqualTo(null));
        var friendList = await Service.GetAllFriends(userDb.Id);
        Assert.That(friendList.Count() == 10);
        
        friendList = await Service.FindFriendByNameSurname(userDb.Id, "Name ");
        Assert.That(friendList.Count() == 10);
    }
}