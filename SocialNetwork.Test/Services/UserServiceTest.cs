using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Web;

namespace SocialNetwork.Test.Services;

public class UserServiceTest : DefaultServiceTest<IUserService, UserService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateUser_SameData_ShouldFail()
    {
        var user = new UserModel()
        {
            Login = "TestLogin1",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "1@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);
        Assert.ThrowsAsync<AlreadyLoginAndEmailException>(async () => await Service.CreateUserAsync(user));
    }


    [Test]
    public async Task CreateUser_WithCorrectData_Success()
    {
        var user = new UserModel()
        {
            Login = "TestLogin2",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "2@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        var createdUser = await Service.GetUserByLogin(user.Login);

        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
    }

    [Test]
    public async Task CreateUserAndGetWithIncorrectId_ShouldFail()
    {
        var user = new UserModel()
        {
            Login = "TestLogin3",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "3@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<UserNotFoundException>(async () => await Service.GetByIdAsync(10));
    }

    [Test]
    public async Task UpdateUser_UserFound_ReturnsUpdatedUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin4",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "4@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin(user.Login);

        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));


        user.Profile.Email = "anotherMail@gmail.com";
        user.Profile.Name = "AnotherName";
        user.Id = createdUser.Id;
        await Service.UpdateUserAsync(user.Id, user);

        createdUser = await Service.GetByIdAsync(user.Id);

        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
        Assert.That(createdUser!.Profile.Name, Is.EqualTo(user.Profile.Name));
    }

    [Test]
    public async Task UpdateUser_UserNotFound_ThrowsUserNotFoundException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin4",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "4@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.UpdateUserAsync(222, user));
    }

    [Test]
    public async Task DeleteUser_UserFound_DeletesUserSuccessfully()
    {
        var user = new UserModel()
        {
            Login = "TestLogin5",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "5@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        await Service.DeleteUserAsync(1);
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await Service.DeleteUserAsync(1));
    }

    [Test]
    public async Task DeleteUser_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await Service.DeleteUserAsync(1));
    }

    [Test]
    public async Task AddAuthorizationValue_UserFoundNullAuthorizationInfo_AddsRefreshTokenSuccessfully()
    {
        var user = new UserModel()
        {
            Login = "TestLogin6",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "6@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin(user.Login);

        Assert.That(createdUser!.AuthorizationInfo, Is.EqualTo(null));
        await Service.AddAuthorizationValueAsync(createdUser!, "", LoginType.LocalSystem);
        createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo.RefreshToken is "");
        Assert.That(createdUser!.AuthorizationInfo, Is.Not.EqualTo(null));
    }

    [Test]
    public async Task AddAuthorizationValue_UserFoundWithAuthorizationInfo_UpdatesRefreshTokenSuccessfully()
    {
        var user = new UserModel()
        {
            Login = "TestLogin7",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "23@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.EqualTo(null));
        await Service.AddAuthorizationValueAsync(createdUser!, "1111", LoginType.LocalSystem);

        var createdUser2 = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser2!.AuthorizationInfo, Is.Not.EqualTo(null));
        await Service.AddAuthorizationValueAsync(createdUser2!, "2222", LoginType.LocalSystem);

        Assert.That(createdUser2!.AuthorizationInfo?.RefreshToken != createdUser!.AuthorizationInfo?.RefreshToken);
    }

    [Test]
    public async Task AddAuthorizationValue_UserFoundWithAuthorizationInfo_ThrowsTimeoutException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin8",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "24@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.EqualTo(null));
        await Service.AddAuthorizationValueAsync(createdUser!, "1111", LoginType.LocalSystem,
            DateTime.Now.AddHours(-26));

        createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.Not.EqualTo(null));

        Assert.ThrowsAsync<TimeoutException>(async () =>
            await Service.AddAuthorizationValueAsync(createdUser!, "2222", LoginType.LocalSystem));
        createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.EqualTo(null));
    }

    [Test]
    public async Task AddAuthorizationValue_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.AddAuthorizationValueAsync(new UserModel(), "", LoginType.LocalSystem));
        await Task.CompletedTask;
    }

    [Test]
    public async Task GetUserByLoginAndPassword_UserFound_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin9",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "25@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);
        Assert.That(Service.GetUserByLoginAndPasswordAsync(user.Login, user.Password), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task GetUserByLoginAndPassword_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByLoginAndPasswordAsync("user.Login", " user.Password"));
        await Task.CompletedTask;
    }

    [Test]
    public async Task GetUserByLoginAndPassword_IncorrectPassword_ThrowsUserNotFoundException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin10",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "62@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<WrongLoginOrPasswordException>(async ()
            => await Service.GetUserByLoginAndPasswordAsync(user.Login, "wrong password"));
    }

    [Test]
    public async Task GetUserByRefreshToken_UserFound_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin11",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "72@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin(user.Login);
        await Service.AddAuthorizationValueAsync(createdUser!,
            "cNJPGDP69Z/fsk6Wm5rP+02Jl+SSgxPPckvk/OKY1hc=-1098260020", LoginType.LocalSystem);
        Assert.That(await Service.GetUserByRefreshTokenAsync("cNJPGDP69Z/fsk6Wm5rP+02Jl+SSgxPPckvk/OKY1hc=-1098260020"),
            Is.Not.EqualTo(null));
    }

    [Test]
    public Task GetUserByRefreshToken_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByRefreshTokenAsync("RefreshToken"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task GetUserByEmail_UserFound_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin12",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "8@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        Assert.That(Service.GetUserByEmail("limpopo923@gmail.com") is not null);
    }

    [Test]
    public async Task GetUserByEmail_UserNotFound_ThrowsUserNotFoundException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin13",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "9@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByEmail("wrongEmail@gmail.com"));
    }

    [Test]
    public async Task GetUserByLogin_UserFound_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin14",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "10@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        Assert.That(Service.GetUserByEmail("TestLogin") is not null);
    }

    [Test]
    public async Task GetUserByLogin_UserNotFound_ThrowsUserNotFoundException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin15",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "11@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByLogin("wrongLogin"));
    }

    [Test]
    public async Task LogOut_DeleteAuthorizationInfo_ReturnNull()
    {
        var user = new UserModel()
        {
            Login = "TestLogin16",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "12@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        var createdUser = await Service.GetUserByLogin("TestLogin16");
        await Service.AddAuthorizationValueAsync(createdUser!, "123", LoginType.LocalSystem);
        createdUser = await Service.GetUserByLogin("TestLogin16");

        if (createdUser?.AuthorizationInfo is not null)
            await Service.LogOutAsync(createdUser.Id);
        createdUser = await Service.GetUserByLogin("TestLogin16");

        Assert.That(createdUser!.AuthorizationInfo is null);
    }

    [Test]
    public async Task LogOut_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.LogOutAsync(1));
        await Task.CompletedTask;
    }

    [Test]
    public async Task LogOut_AuthorizationInfoTrue_ThrowsNullReferenceException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin17",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "13@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetUserByLogin("TestLogin17");
        Assert.ThrowsAsync<NullReferenceException>(async ()
            => await Service.LogOutAsync(createdUser!.Id));
    }
}