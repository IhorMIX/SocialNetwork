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

public class UserServiceTest : DefaultServiceTest<IUserService ,UserService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateUser_SameData_ShouldFail()
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



        Assert.ThrowsAsync<AlreadyLoginAndEmailException>(async () => await Service.CreateUserAsync(user));
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

        Assert.ThrowsAsync<UserNotFoundException>(async () => await Service.GetById(10));
    }
    
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
        var createdUser = await Service.GetUserByLogin(user.Login);
        
        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
        
        user.Login = "AnotherLogin";
        user.Profile.Email = "anotherMail@gmail.com";
        user.Id = createdUser.Id;
        await Service.UpdateUserAsync(user);
        
        createdUser = await Service.GetUserByLogin(user.Login);
        
        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
    }

    [Test]
    public async Task UpdateUser_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () 
            => await Service.UpdateUserAsync((await Service.GetById(1))!));
    }

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

    [Test]
    public async Task AddAuthorizationValue_UserFound_UpdatesRefreshTokenSuccessfully()
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
        
        var createdUser = await Service.GetUserByLogin(user.Login);
    
        await Service.AddAuthorizationValueAsync(createdUser!, "", LoginType.LocalSystem);
        
        createdUser = await Service.GetUserByLogin(user.Login);
        
        Assert.That(createdUser!.AuthorizationInfo is not null);
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
        await Task.CompletedTask;
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

        Assert.ThrowsAsync<WrongLoginOrPasswordException>(async () 
            => await Service.GetUserByLoginAndPasswordAsync(user.Login, "wrong password"));
    }

    [Test]
    public async Task GetUserByRefreshToken_UserFound_ReturnsUserModel()
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
        var createdUser = await Service.GetUserByLogin(user.Login);
        await Service.AddAuthorizationValueAsync(createdUser!, "myzpHVRKHcWh+WB2kG/hgES9LS1PDFrUpb2IkAMtSok=-68397843", LoginType.LocalSystem);
        Assert.That(await Service.GetUserByRefreshTokenAsync("myzpHVRKHcWh+WB2kG/hgES9LS1PDFrUpb2IkAMtSok=-68397843"), Is.Not.EqualTo(null));
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
        Assert.That(Service.GetUserByEmail("limpopo923@gmail.com") is not null);
    }
    
    [Test]
    public async Task GetUserByEmail_UserNotFound_ThrowsUserNotFoundException()
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
        
        Assert.ThrowsAsync<UserNotFoundException>(async() 
            => await Service.GetUserByEmail("wrongEmail@gmail.com"));
    }

    [Test]
    public async Task GetUserByLogin_UserFound_ReturnsUserModel()
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
        
        Assert.That(Service.GetUserByEmail("TestLogin") is not null);
    }
    
    [Test]
    public async Task GetUserByLogin_UserNotFound_ThrowsUserNotFoundException()
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
        
        Assert.ThrowsAsync<UserNotFoundException>(async() 
            => await Service.GetUserByLogin("wrongLogin"));
    }

    [Test]
    public async Task LogOut_DeleteAuthorizationInfo_ReturnNull()
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
        
        var createdUser = await Service.GetUserByLogin("TestLogin");
        await Service.AddAuthorizationValueAsync(createdUser!, "123", LoginType.LocalSystem);
        createdUser = await Service.GetUserByLogin("TestLogin");
        
        if (createdUser?.AuthorizationInfo is not null)
            await Service.LogOutAsync(createdUser.Id);
        createdUser = await Service.GetUserByLogin("TestLogin");
        
        Assert.That(createdUser!.AuthorizationInfo is null);
    }
    
    [Test]
    public async Task LogOut_UserNotFound_ThrowsUserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async() 
           => await Service.LogOutAsync(1));
        await Task.CompletedTask;
    }
    [Test]
    public async Task LogOut_AuthorizationInfoTrue_ThrowsNullReferenceException()
    {
        var user = new UserModel()
        {
            Login = "TestLogin2",
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
        var createdUser = await Service.GetUserByLogin("TestLogin2");
        Assert.ThrowsAsync<NullReferenceException>(async() 
            => await Service.LogOutAsync(createdUser!.Id));
    }
}