using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;
using System.Linq;

public class FriendServiceTest : DefaultServiceTest<IFriendshipService, FriendshipService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatMemberRepository, ChatMemberRepository>();

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IRequestService, RequestService>();

        services.AddScoped<IBannedUserListRepository, BannedUserListRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupService, GroupService>();

        base.SetUpAdditionalDependencies(services);
    }
    
    [Test]
    public async Task CreateFriendship_UsersFound_createdFriendship()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.AddFriendshipAsync(user1!.Id, user2!.Id);
        Assert.That(Service.GetByIdAsync(1123124), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task DeleteFriend_FriendFound_DeletedFriend()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByIdAsync(1), Is.Not.EqualTo(null));
    }
    [Test]
    public async Task DeleteFriend_FriendNotFound_DeletedFriend()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        Assert.ThrowsAsync<FriendNotFoundException>(async () => await Service.GetByIdAsync(1));
    }

    [Test]
    public async Task GetAllFriends_UserFound_ReturnFriends()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        Assert.That(user1, Is.Not.EqualTo(null));

        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user2, Is.Not.EqualTo(null));

        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user3, Is.Not.EqualTo(null));
        
        await Service.AddFriendshipAsync(createdUser1!.Id,createdUser2!.Id);
        
        await Service.AddFriendshipAsync(createdUser1!.Id,createdUser3!.Id);
        
        Assert.That(
            Service.GetAllFriends(user1.Id, paginationModel), 
            Is.Not.EqualTo(null));
        
        var friendList =await Service.GetAllFriends(user1.Id, paginationModel);
        Assert.That(friendList.Data.Count() == 2);
    }
    
    [Test]
    public async Task GetFriendByEmail_UserFound_ReturnFriend()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        var userDb1 = await userService.GetUserByLogin(user1.Login);
        var userDb2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        
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
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser = await userService.GetUserByLogin(user.Login);
        Assert.That(user, Is.Not.EqualTo(null));
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        var userList = new List<UserModel>();
        
        for (int i = 0; i < 6; i++)
            userList.Add(await UserModelHelper.CreateTestDataAsync(userService));
        
        
        for (int i = 0; i < 6; i++)
            await Service.AddFriendshipAsync(createdUser!.Id, 
                (await userService.GetUserByLogin(userList[i].Login))!.Id);
        
        Assert.That(Service.GetAllFriends(createdUser!.Id, paginationModel), Is.Not.EqualTo(null));
        var friendList = await Service.GetAllFriends(createdUser.Id, paginationModel);
        Assert.That(friendList.Data.Count() == 6);
        friendList = await Service.FindFriendByNameSurname(createdUser.Id, paginationModel, "Test ");
        Assert.That(friendList.Data.Count() == 6);
    }
}