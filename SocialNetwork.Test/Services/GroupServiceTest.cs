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

public class GroupServiceTest : BaseMessageTestService<IGroupService, GroupService>
{

    [Test]
    public async Task CreateGroup_Ok_GroupCreated()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.JoinGroup(group.Id, user2.Id);

        Assert.That((await Service.GetGroupMembers(user1.Id, paginationModel, group.Id)).Data.Count() == 2);
    }

    [Test]
    public async Task CreateGroup_UserJoinedTwice_AsssertException()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.JoinGroup(group.Id, user2.Id);

        Assert.ThrowsAsync<GroupMemberException>(async () => await Service.JoinGroup(group.Id, user2.Id));
    }

    [Test]
    public async Task CreateGroup_UserJoinAndLeave_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.JoinGroup(group.Id, user2.Id);
        await Service.LeaveGroup(group.Id, user2.Id);
        Assert.That((await Service.GetGroupMembers(user1.Id, paginationModel, group.Id)).Data.Count() == 1);
    }

    [Test]
    public async Task CreateGroup_UserJoinAndAdminLeave_AsssertException()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.JoinGroup(group.Id, user2.Id);
        Assert.ThrowsAsync<CreatorCantLeaveException>(async () => await Service.LeaveGroup(group.Id, user1.Id));
    }

    [Test]
    public async Task CreateGroup_UserJoinCreatorKickHim_Success()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.JoinGroup(group.Id, user2.Id);
        await Service.KickMember(user1.Id, group.Id, user2.Id);

        Assert.That((await Service.GetGroupMembers(user1.Id, paginationModel, group.Id)).Data.Count() == 1);
    }

    [Test]
    public async Task CreateGroups_GetGroups_AssertSucces()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        Assert.That(user1, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });
        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group2",
            Logo = "null",
        });
        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group3",
            Logo = "null",
        });

        var group = await Service.GetAllGroups(user1.Id, paginationModel);
        Assert.That(group.Data.Count() == 3);
    }

    [Test]
    public async Task TryToLeave_MakeHost_Leave()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "GroupLeave",
            Logo = "null",
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "GroupLeave")).Data.First();
        await Service.JoinGroup(group.Id, user2.Id);
        
        Assert.ThrowsAsync<CreatorCantLeaveException>(async () => await Service.LeaveGroup(group.Id, user1.Id));

        await Service.MakeHost(user1.Id, group.Id, user2.Id);
        await Service.LeaveGroup(group.Id, user1.Id);

        Assert.That((await Service.GetGroupMembers(user2.Id, paginationModel, group.Id)).Data.Count() == 1);
    }

    [Test]
    public async Task CreateGroup_EditGroup_AssertSuccess()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        await Service.CreateGroup(user1.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });
        GroupModel groupModel = new GroupModel
        {
            Name = "GroupEdited",
            Logo = "null",
        };
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var groupList = await Service.FindGroupByName(user1.Id, paginationModel, "Group1");
        var group = groupList.Data.First();

        await Service.EditGroup(user1.Id, group.Id, groupModel);

        Assert.That((await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.Count() == 0);
        Assert.That((await Service.FindGroupByName(user1.Id, paginationModel, "GroupEdited")).Data.Count() == 1);
    }


    [Test]
    public async Task CreateRole_EditRole_RoleEdited()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group is not null);

        await Service.JoinGroup(group!.Id, user2.Id);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 2);

        await Service.AddRole(user1.Id, group.Id, new RoleGroupModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });

        var role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, group.Id, role!.Id, new List<int>() { user2.Id });

        role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Any(m => m.RoleGroup.Any(r => r.RoleName == role.RoleName) && m.User.Login == user2.Login));

        role.RoleAccesses.Clear();

        await Service.EditRole(user1.Id, group.Id, role.Id, role);
        role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        Assert.That(!role.RoleAccesses.Contains(GroupAccess.DelGroupMembers));

        role.RoleName = "Role21";
        role.RoleColor = "black1";
        role.RoleAccesses.Add(GroupAccess.DelGroupMembers);
        await Service.EditRole(user1.Id, group.Id, role.Id, role);

        role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(role.RoleName != "Role2" &&
                    group.GroupMembers!.Any(c => c.RoleGroup.Any(r => r.RoleName != "Role2")));
        Assert.That(role.RoleAccesses.Contains(GroupAccess.DelGroupMembers));
    }

    [Test]
    public async Task CreateRole_UnsetRole_AssertSuccess()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group is not null);

        await Service.JoinGroup(group!.Id, user2.Id);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 2);

        await Service.AddRole(user1.Id, group.Id, new RoleGroupModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });

        var role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, group.Id, role!.Id, new List<int>() { user2.Id });

        role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Any(m => m.RoleGroup.Any(r => r.RoleName == role.RoleName) && m.User.Login == user2.Login));

        role.RoleAccesses.Clear();

        await Service.UnSetRole(user1.Id, group.Id, role!.Id, new List<int>() { user2.Id });
        role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        
        var check = await Service.GetGroupMembers(user1.Id,paginationModel, group.Id, role.Id); 

       Assert.That(check.Data.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task CreateRole_DeleteRole_AssertSuccess()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group is not null);

        await Service.JoinGroup(group!.Id, user2.Id);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 2);

        await Service.AddRole(user1.Id, group.Id, new RoleGroupModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });

        var role = (await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id)).Data.First();
        Assert.That(await Service.GetRoleById(user1.Id, group.Id, role.Id) is not null);

        await Service.DelRole(user1.Id,group.Id,role!.Id);

        var check = await Service.GetAllGroupRoles(user1.Id, paginationModel, group.Id);

        Assert.That(check.Data.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task CreateGroup_BanMember_AssertSuccess()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group is not null);

        await Service.JoinGroup(group!.Id, user2.Id);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 2);

        await Service.BanGroupMember(user1.Id,group.Id, user2.Id,"toxic");

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 1);
    }

    [Test]
    public async Task CreateGroup_UnBanMember_AssertSuccess()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);

        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);

        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        await Service.CreateGroup(user1!.Id, new GroupModel
        {
            Name = "Group1",
            Logo = "null",
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group is not null);

        await Service.JoinGroup(group!.Id, user2.Id);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 2);

        await Service.BanGroupMember(user1.Id, group.Id, user2.Id, "toxic");
        Assert.That((await Service.GetAllBannedUser(user1.Id,group.Id, paginationModel)).Data.Count() == 1);

        group = (await Service.FindGroupByName(user1.Id, paginationModel, "Group1")).Data.First();
        Assert.That(group.GroupMembers!.Count == 1);

        await Service.UnBanGroupMember(user1.Id, group.Id, user2.Id);

        Assert.That((await Service.GetAllBannedUser(user1.Id, group.Id, paginationModel)).Data.Count() == 0);
    }

}
