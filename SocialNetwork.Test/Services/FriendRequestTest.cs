using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class FriendRequestTest : DefaultServiceTest<IFriendRequestService, FriendRequestService>
{

    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
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
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateFriendRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.SendRequest(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByUsersId(createdUser1.Id, createdUser2.Id), Is.Not.EqualTo(null));
    }
    
    [Test]
    public async Task AcceptFriendRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.SendRequest(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByUsersId(createdUser1.Id, createdUser2.Id), Is.Not.EqualTo(null));
        
        var request = await Service.GetByUsersId(createdUser1.Id, createdUser2.Id);
        await Service.AcceptRequest(request.ReceiverId, request.Id);
        Assert.That(friendService.FindFriendByNameSurname(createdUser1.Id, "Test"), Is.Not.EqualTo(null));
        Assert.That(friendService.FindFriendByNameSurname(createdUser2.Id, "Test"), Is.Not.EqualTo(null));
        Assert.ThrowsAsync<FriendRequestException>(() => Service.GetByUsersId(createdUser1.Id, createdUser2.Id));
    }
    [Test]
    public async Task CancelFriendRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.SendRequest(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByUsersId(createdUser1.Id, createdUser2.Id), Is.Not.EqualTo(null));
        
        var request = await Service.GetByUsersId(createdUser1.Id, createdUser2.Id);
        await Service.CancelRequest(request.ReceiverId, request.Id);
        Assert.That(!(await friendService.FindFriendByNameSurname(createdUser1.Id, "Test")).Any());
        Assert.That(!(await friendService.FindFriendByNameSurname(createdUser2.Id, "Test")).Any());
        Assert.ThrowsAsync<FriendRequestException>(() => Service.GetByUsersId(createdUser1.Id, createdUser2.Id));
    }
    
    [Test]
    public async Task GetAllRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        var createdUser3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));

        
        await Service.SendRequest(createdUser2!.Id, createdUser1!.Id);
        await Service.SendRequest(createdUser3!.Id, createdUser1!.Id);
        Assert.That(Service.GetByUsersId(createdUser2.Id, createdUser1.Id), Is.Not.EqualTo(null));
        Assert.That(Service.GetByUsersId(createdUser3.Id, createdUser1.Id), Is.Not.EqualTo(null));
        
        var request = await Service.GetAllIncomeRequest(createdUser1.Id);
        Assert.That(request.Count() == 2);
    }

    [Test]
    public async Task CreateFriendRequest_RequestAlreadyExists_FailedRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.SendRequest(user1.Id, user2.Id);
        Assert.ThrowsAsync<FriendRequestException>(async () => await Service.SendRequest(user1!.Id, user2!.Id));
    }

    [Test]
    public async Task CreateFriendRequest_MirrorRequestAlreadyExists_FailedRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.SendRequest(user1.Id, user2.Id);
        Assert.ThrowsAsync<FriendRequestException>(async () => await Service.SendRequest(user2!.Id, user1!.Id));
    }

    [Test]
    public async Task CreateFriendRequest_RequestToBannedUser_FailedRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var blacklistService = ServiceProvider.GetRequiredService<IBlackListService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var banneduser = await UserModelHelper.CreateTestDataAsync(userService);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(banneduser, Is.Not.EqualTo(null));

        await blacklistService.AddUserToBlackListAsync(user1.Id, banneduser.Id);
        Assert.ThrowsAsync<FriendRequestException>(async () => await Service.SendRequest(user1!.Id, banneduser!.Id));
    }
}