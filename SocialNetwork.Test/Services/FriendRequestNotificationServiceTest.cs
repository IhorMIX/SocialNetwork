using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class FriendRequestNotificationServiceTest : DefaultServiceTest<INotificationService, NotificationService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateNewFriendRequest_CheckNotificationEntity_OK()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var fiendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
        await fiendRequestService.SendRequest(createdUser1!.Id, createdUser2!.Id);
        var notification = await Service.GetByUserId(createdUser2!.Id);
        var notificationModels = notification.ToList();
        Assert.That(notificationModels.First().UserId == createdUser2!.Id);
        Assert.That(notificationModels.First().IsRead is false);
    }

    [Test]
    public async Task CreateNotification_ChangeReadProp_DeleteNotification()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var fiendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
        await fiendRequestService.SendRequest(createdUser1!.Id, createdUser2!.Id);
        
        var fRequest = await fiendRequestService.GetByUsersId(createdUser1.Id, createdUser2.Id);
        var notifications = await Service.GetByUserId(createdUser2.Id);
        var notification = notifications.First();
        await Service.ReadNotification(createdUser2.Id, notification!.Id);
        
        notification = await Service.GetByIdAsync(notification.Id, NotificationType.FriendRequest);
        Assert.That(notification!.IsRead);
    
        await Service.RemoveNotification(createdUser2.Id, notification.Id);
        Assert.ThrowsAsync<NotificationNotFoundException>(async () => await Service.GetByIdAsync(notification.Id));
    }
}