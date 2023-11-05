using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.Test.Helpers;
using System;

namespace SocialNetwork.Test.Services;

public class BlackListServiceTest : BaseMessageTestService<IBlackListService, BlackListService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        base.SetUpAdditionalDependencies(services);
    }
    [Test]
    public async Task AddUserToBlackList_UserFound_AddedToBlackList()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var justUser = await UserModelHelper.CreateTestDataAsync(userService);
        var wantToBanUser = await UserModelHelper.CreateTestDataAsync(userService);
        justUser = await userService.GetByIdAsync(justUser.Id);
        wantToBanUser = await userService.GetByIdAsync(wantToBanUser.Id);
        Assert.That(justUser, Is.Not.EqualTo(null));
        Assert.That(wantToBanUser, Is.Not.EqualTo(null));

        await Service.AddUserToBlackListAsync(justUser!.Id, wantToBanUser!.Id);
        var userInBlackList = await Service.IsBannedUser(justUser.Id, wantToBanUser.Id);
        Assert.That(userInBlackList, Is.True);

    }
    [Test]
    public async Task DeleteUserFromBlackList_UserFound_RemovedFromBlackList()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var justUser = await UserModelHelper.CreateTestDataAsync(userService);
        var wantToBanUser = await UserModelHelper.CreateTestDataAsync(userService);

        await Service.AddUserToBlackListAsync(justUser.Id, wantToBanUser.Id);
        var userInBlackList = await Service.IsBannedUser(justUser.Id, wantToBanUser.Id);
        Assert.That(userInBlackList, Is.True);

        await Service.DeleteUserFromBlackListAsync(justUser.Id, wantToBanUser.Id);
        var userInBlackListAfterRemoval = await Service.IsBannedUser(justUser.Id, wantToBanUser.Id);
        Assert.That(userInBlackListAfterRemoval, Is.False);
    }
    [Test]
    public async Task FindBannedUserByNameSurname_UserFound_ReturnsMatchingUsers()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var blackListService = ServiceProvider.GetRequiredService<IBlackListService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var wantToBanUser = await UserModelHelper.CreateTestDataAsync(userService);


        await blackListService.AddUserToBlackListAsync(user1.Id, wantToBanUser.Id);

        var nameSurname = $"{wantToBanUser.Profile.Name} {wantToBanUser.Profile.Surname}";
        var bannedUsers = await blackListService.FindBannedUserByNameSurname(user1.Id, nameSurname);

        Assert.That(bannedUsers, Is.Not.Empty);

    }

    [Test]
    public async Task CreateFriendship_AddToBlackList_UserIsBanned()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendshipService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await friendshipService.AddFriendshipAsync(user1!.Id, user2!.Id);
        await Service.AddUserToBlackListAsync(user1!.Id, user2!.Id);
        await friendshipService.DeleteFriendshipAsync(user1!.Id, user2!.Id);
        Assert.Multiple(async() =>
        {
            Assert.That(await Service.IsBannedUser(user1!.Id, user2!.Id), Is.True);
            Assert.That(await friendshipService.IsFriends(user1!.Id, user2!.Id), Is.False);
        });
    }
}