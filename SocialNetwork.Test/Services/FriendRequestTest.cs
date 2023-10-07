using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
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
        
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateFriendRequest()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        await Service.SendRequest(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByUsersId(createdUser1.Id, createdUser2.Id), Is.Not.EqualTo(null));
    }
    
    [Test]
    public async Task AcceptFriendRequest()
    {
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
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
        var user1 = await UserModelHelper.CreateTestData();
        var user2 = await UserModelHelper.CreateTestData();
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
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

        var user1 = await UserModelHelper.CreateTestData();
        await userService.CreateUserAsync(user1);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        
        var user2 = await UserModelHelper.CreateTestData();
        await userService.CreateUserAsync(user2);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var user3 = await UserModelHelper.CreateTestData();
        await userService.CreateUserAsync(user3);
        var createdUser3 = await userService.GetUserByLogin(user3.Login);
        
        await Service.SendRequest(createdUser2!.Id, createdUser1!.Id);
        await Service.SendRequest(createdUser3!.Id, createdUser1!.Id);
        Assert.That(Service.GetByUsersId(createdUser2.Id, createdUser1.Id), Is.Not.EqualTo(null));
        Assert.That(Service.GetByUsersId(createdUser3.Id, createdUser1.Id), Is.Not.EqualTo(null));
        
        var request = await Service.GetAllIncomeRequest(createdUser1.Id);
        Assert.That(request.Count() == 2);
    }
    
}