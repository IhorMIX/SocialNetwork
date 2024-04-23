using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class RequestServiceTest : DefaultServiceTest<IRequestService, RequestService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();

        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendshipService, FriendshipService>();

        services.AddScoped<IRequestService , RequestService>();
        services.AddScoped<IRequestRepository , RequestRepository>();

        services.AddScoped<IBannedUserListRepository, BannedUserListRepository>();
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task SendFriendRequest_AcceptFriendRequest_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var friendrequestModel = new FriendRequestModel
        {
            SenderId = user1.Id,
            ToUserId = user2.Id,
        };

        var requestId = await Service.SendFriendRequestAsync(friendrequestModel);
        Assert.That(Service.GetAllSentFriendRequest(user1.Id, paginationModel), Is.Not.EqualTo(null));

        await Service.AcceptFriendRequest(user2.Id, requestId);
        Assert.That(friendService.IsFriends(user1.Id, user2.Id), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task SendGroupRequest_AcceptGroupRequest_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var groupService = ServiceProvider.GetRequiredService<IGroupService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await groupService.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            IsPrivate = true,
            Description = "hi",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await groupService.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        var groupRequestModel = new GroupRequestModel
        {
            SenderId = user2.Id,
            ToGroupId = group.Id,
        };

        var requestId = await Service.SendGroupRequestAsync(groupRequestModel);
        Assert.That(Service.GetAllSentGroupRequest(user2.Id, paginationModel), Is.Not.EqualTo(null));

        await Service.AcceptGroupRequest(user1.Id, requestId);
        Assert.That((await groupService.GetGroupMembers(user1.Id, paginationModel, group.Id)).Data.Count() == 2);
    }

    [Test]
    public async Task SendFriendRequest_CancelFriendRequest_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var friendrequestModel = new FriendRequestModel
        {
            SenderId = user1.Id,
            ToUserId = user2.Id,
        };

        var requestId = await Service.SendFriendRequestAsync(friendrequestModel);
        Assert.That(Service.GetAllSentFriendRequest(user1.Id, paginationModel), Is.Not.EqualTo(null));

        Assert.That(Service.GetAllIncomeFriendRequest(user2.Id, paginationModel), Is.Not.EqualTo(null));

        await Service.CancelFriendRequest(user2.Id, requestId);
        Assert.That(friendService.IsFriends(user1.Id, user2.Id), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task SendGroupRequest_CancelGroupRequest_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var groupService = ServiceProvider.GetRequiredService<IGroupService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await groupService.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            IsPrivate = true,
            Description = "hi",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await groupService.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        var groupRequestModel = new GroupRequestModel
        {
            SenderId = user2.Id,
            ToGroupId = group.Id,
        };

        var requestId = await Service.SendGroupRequestAsync(groupRequestModel);
        Assert.That(Service.GetAllSentGroupRequest(user2.Id, paginationModel), Is.Not.EqualTo(null));

        Assert.That(Service.GetAllIncomeGroupRequest(user1.Id, paginationModel), Is.Not.EqualTo(null));

        await Service.CancelGroupRequest(user1.Id, requestId);
        Assert.That((await groupService.GetGroupMembers(user1.Id, paginationModel, group.Id)).Data.Count() == 1);
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

        var friendrequestModel = new FriendRequestModel
        {
            Id = 1,
            SenderId = user1.Id,
            ToUserId = user2.Id,
        };

        await Service.SendFriendRequestAsync(friendrequestModel);
        Assert.ThrowsAsync<RequestException>(async () => await Service.SendFriendRequestAsync(friendrequestModel));
    }

    [Test]
    public async Task CreateGroupRequest_RequestAlreadyExists_FailedRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var groupService = ServiceProvider.GetRequiredService<IGroupService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        await groupService.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            IsPrivate = true,
            Description = "hi",
            Logo = "null",
        });

        var groupList = await groupService.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        var groupRequestModel = new GroupRequestModel
        {
            SenderId = user2.Id,
            ToGroupId = group.Id,
        };

        await Service.SendGroupRequestAsync(groupRequestModel);
        Assert.ThrowsAsync<GroupRequestException>(async () => await Service.SendGroupRequestAsync(groupRequestModel));
    }

    [Test]
    public async Task CreateGroupRequest_GroupIsNotPrivate_FailedRequest()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var groupService = ServiceProvider.GetRequiredService<IGroupService>();
        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();

        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        await groupService.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            IsPrivate = false,
            Description = "hi",
            Logo = "null",
        });

        var groupList = await groupService.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        var groupRequestModel = new GroupRequestModel
        {
            SenderId = user2.Id,
            ToGroupId = group.Id,
        };

        Assert.ThrowsAsync<GroupRequestException>(async () => await Service.SendGroupRequestAsync(groupRequestModel));
    }
}
