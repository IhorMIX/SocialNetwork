using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.Test.Services;

public class UserServiceTest : DefaultServiceTest<IUserService ,UserService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateUser_WithCorrectData_Success()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        var createdUser = await Service.GetById(1);

        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
    }

    [Test]
    public async Task CreateUserAndGetWithIncorrectId_ShouldFail()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<UserNotFoundException>(async () => await Service.GetById(3));
    }
    
    //----------------------------update user test---------------------------- 
    //Test_UpdateUserAsync_UserFound_ReturnsUpdatedUserModel
    //Test_UpdateUserAsync_UserNotFound_ThrowsUserNotFoundException
    [Test]
    public async Task UpdateUser_UserFound_ReturnsUpdatedUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        var createdUser = await Service.GetById(1);
        
        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
        
        user.Login = "AnotherLogin";
        user.Profile.Email = "anotherMail@gmail.com";
        await Service.UpdateUserAsync((await Service.GetById(1))!);
        
        createdUser = await Service.GetById(1);
        
        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
    }

    [Test]
    public async Task UpdateUser_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () 
            => await Service.UpdateUserAsync((await Service.GetById(1))!));
    }
    
    
    //----------------------------DeleteUser Test---------------------------- OK
    //Test_DeleteUserAsync_UserFound_DeletesUserSuccessfully
    //Test_DeleteUserAsync_UserNotFound_ThrowsUserNotFoundException 

    [Test]
    public async Task DeleteUser_UserFound_DeletesUserSuccessfully()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        
        await Service.DeleteUserAsync((await Service.GetById(1))!);
        Assert.ThrowsAsync<UserNotFoundException>(async () => 
            await Service.DeleteUserAsync((await Service.GetById(1))!));
    }    
    [Test]
    public async Task DeleteUser_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () => 
            await Service.DeleteUserAsync((await Service.GetById(1))!));
    }    
    
    
    //----------------------------UpdateRefreshToken Test----------------------------
    //Test_UpdateRefreshTokenAsync_UserFound_UpdatesRefreshTokenSuccessfully
    //Test_UpdateRefreshTokenAsync_UserNotFound_ThrowsUserNotFoundException

    [Test]
    public async Task UpdateRefreshToken_UserFound_UpdatesRefreshTokenSuccessfully()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);
        
        var createdUser = await Service.GetById(1);

        await Service.UpdateRefreshTokenAsync(1, "RefreshToken");
        
        createdUser = await Service.GetById(1);
        
        Assert.That(createdUser!.AuthorizationInfo.RefreshToken, 
            Is.EqualTo(user.AuthorizationInfo.RefreshToken));
    }

    [Test]
    public async Task UpdateRefreshToken_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () 
            => await Service.UpdateRefreshTokenAsync(20, "refreshToken"));
    }
    
    //----------------------------GetUserByLoginAndPassword Test---------------------------- OK
    //Test_GetUserByLoginAndPasswordAsync_UserFound_ReturnsUserModel
    //Test_GetUserByLoginAndPasswordAsync_UserNotFound_ThrowsUserNotFoundException
    //Test_GetUserByLoginAndPasswordAsync_IncorrectPassword_ThrowsUserNotFoundException

    [Test]
    public async Task GetUserByLoginAndPassword_UserFound_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);
        Assert.That(Service.GetUserByLoginAndPasswordAsync(user.Login, user.Password) != null);
    }

    [Test]
    public async Task GetUserByLoginAndPassword_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () 
            => await Service.GetUserByLoginAndPasswordAsync("user.Login"," user.Password"));
    }

    [Test]
    public async Task GetUserByLoginAndPassword_IncorrectPassword_ThrowsUserNotFoundException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };

        await Service.CreateUserAsync(user);

        Assert.ThrowsAsync<WrongPasswordException>(async () 
            => await Service.GetUserByLoginAndPasswordAsync(user.Login, "wrong password"));
    }
    
    //----------------------------GetUserByRefreshToken----------------------------
    //Test_GetUserByRefreshTokenAsync_UserFound_ReturnsUserModel
    //Test_GetUserByRefreshTokenAsync_UserNotFound_ThrowsUserNotFoundException

    [Test]
    public async Task GetUserByRefreshToken_UserFount_ReturnsUserModel()
    {
        var user = new UserModel()
        {
            Login = "TestLogin",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "limpopo923@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        await Service.CreateUserAsync(user);

        Assert.That(Service.GetUserByRefreshTokenAsync(user.AuthorizationInfo.RefreshToken) != null);
    }
    
    [Test]
    public Task GetUserByRefreshToken_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () 
            => await Service.GetUserByRefreshTokenAsync("RefreshToken"));
        return Task.CompletedTask;
    }
    
}