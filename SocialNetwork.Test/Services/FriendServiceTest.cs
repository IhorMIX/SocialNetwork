using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.BL.Services;
namespace SocialNetwork.Test.Services;
using System.Linq;

public class FriendServiceTest : DefaultServiceTest<IFriendshipService, FriendshipService>
{
    
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateFriendship_UsersFound_createdFriendship()
    {
        var user1 = new UserModel()
        {
            Login = "User1",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User1@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var user2 = new UserModel()
        {
            Login = "User2",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User2@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        user1 = await userService.GetUserByLogin(user1.Login);
        await userService.CreateUserAsync(user2);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));


        
        await Service.AddFriendshipAsync(user1!.Id, user2!.Id);
        Assert.That(Service.GetByIdAsync(1), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task DeleteFriend_FriendFound_DeletedFriend()
    {
        var user1 = new UserModel()
        {
            Login = "User228",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User228@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var user2 = new UserModel()
        {
            Login = "User211",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User211@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        await userService.CreateUserAsync(user2);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        await Service.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);
        Assert.That(Service.GetByIdAsync(1), Is.Not.EqualTo(null));
    }

    [Test]
    public async Task GetAllFriends_UserFound_ReturnFriends()
    {
        var user1 = new UserModel()
        {
            Login = "User123",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User123@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        
        user1.Login = "User231";
        user1.Profile.Email = "USER232@gmail.com";
        await userService.CreateUserAsync(user1);
        
        user1.Login = "User321";
        user1.Profile.Email = "USER321@gmail.com";
        await userService.CreateUserAsync(user1);

        user1 = await userService.GetUserByLogin("User123");
        var user2 = await userService.GetUserByLogin("User231");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);
        
        user1 = await userService.GetUserByLogin("User123");
        user2 = await userService.GetUserByLogin("User321");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);
        
        Assert.That(
            Service.GetFriendship(await userService.GetUserByLogin("User123")), 
            Is.Not.EqualTo(null));
        
        var friendList =await Service.GetFriendship(await userService.GetUserByLogin("User123"));
        Assert.That(friendList.Count() == 2);
    }
    
    [Test]
    public async Task GetFriendsByEmail_UserFound_ReturnFriend()
    {
        var user1 = new UserModel()
        {
            Login = "User111",
            Password = "TestPassword",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User111@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        await userService.CreateUserAsync(user1);
        
        user1.Login = "User222";
        user1.Profile.Email = "USER222@gmail.com";
        await userService.CreateUserAsync(user1);
        
        user1.Login = "User333";
        user1.Profile.Email = "USER333@gmail.com";
        await userService.CreateUserAsync(user1);
        
        user1 = await userService.GetUserByLogin("User111");
        var user2 = await userService.GetUserByLogin("User222");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);

        user1 = await userService.GetUserByLogin("User333");
        user2 = await userService.GetUserByLogin("User111");
        await Service.AddFriendshipAsync(user1!.Id,user2!.Id);
        
        Assert.That(
            Service.FindFriendByEmail(await userService.GetUserByLogin("User111"), "USER333@gmail.com"), 
            Is.Not.EqualTo(null));
        
        var Friend = await Service.FindFriendByEmail(
            await userService.GetUserByLogin("User111"),"USER333@gmail.com");
        
        var friendUser = await userService.GetUserByLogin("User333");
        
        Assert.That(Friend.Profile.Email == friendUser?.Profile.Email);
        Assert.That(Friend.Login == friendUser?.Login);
    }
}