using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;
using SocialNetwork.Web;

namespace SocialNetwork.Test.Services;

public class UserServiceTest : DefaultServiceTest<IUserService, UserService>
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
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreateUser_SameData_ShouldFail()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.ThrowsAsync<AlreadyLoginAndEmailException>(async () => await Service.CreateUserAsync(user));
    }


    [Test]
    public async Task CreateUser_WithCorrectData_Success()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        var createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.Login, Is.EqualTo(user.Login));
        Assert.That(createdUser!.Profile.Email, Is.EqualTo(user.Profile.Email));
    }

    [Test]
    public async Task CreateUserAndGetWithIncorrectId_ShouldFail()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.ThrowsAsync<UserNotFoundException>(async () => await Service.GetByIdAsync(1532351));
    }

    [Test]
    public async Task UpdateUser_UserFound_ReturnsUpdatedUserModel()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.UpdateUserAsync(222, user));
    }

    [Test]
    public async Task DeleteUser_UserFound_DeletesUserSuccessfully()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        var createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.EqualTo(null));
        await Service.AddAuthorizationValueAsync(createdUser!, "1111", LoginType.LocalSystem,
            DateTime.Now.AddHours(-26));

        createdUser = await Service.GetUserByLogin(user.Login);
        Assert.That(createdUser!.AuthorizationInfo, Is.Not.EqualTo(null));

        Assert.ThrowsAsync<TimeoutException>(async () =>
            await Service.GetUserByRefreshTokenAsync(createdUser.AuthorizationInfo.RefreshToken));
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        
        Assert.ThrowsAsync<WrongLoginOrPasswordException>(async ()
            => await Service.GetUserByLoginAndPasswordAsync(user.Login, "wrong password"));
    }

    [Test]
    public async Task GetUserByRefreshToken_UserFound_ReturnsUserModel()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.That(Service.GetUserByEmail(user.Profile.Email) is not null);
    }

    [Test]
    public async Task GetUserByEmail_UserNotFound_ThrowsUserNotFoundException()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByEmail(user.Profile.Email + "m"));
    }

    [Test]
    public async Task GetUserByLogin_UserFound_ReturnsUserModel()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.That(Service.GetUserByEmail(user.Login) is not null);
    }

    [Test]
    public async Task GetUserByLogin_UserNotFound_ThrowsUserNotFoundException()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await Service.GetUserByLogin(user.Login + "123"));
    }

    [Test]
    public async Task LogOut_DeleteAuthorizationInfo_ReturnNull()
    {
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        var createdUser = await Service.GetUserByLogin(user.Login);
        await Service.AddAuthorizationValueAsync(createdUser!, "123", LoginType.LocalSystem);
        createdUser = await Service.GetUserByLogin(user.Login);

        if (createdUser?.AuthorizationInfo is not null)
            await Service.LogOutAsync(createdUser.Id);
        createdUser = await Service.GetUserByLogin(user.Login);

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
        var user = await UserModelHelper.CreateTestDataAsync(Service);
        var createdUser = await Service.GetUserByLogin(user.Login);
        Assert.ThrowsAsync<NullReferenceException>(async ()
            => await Service.LogOutAsync(createdUser!.Id));
    }
}