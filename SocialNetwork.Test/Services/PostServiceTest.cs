using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class PostServiceTest : DefaultServiceTest<IPostService, PostService>
{
    
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        
        services.AddScoped<IPostRepository, PostRepository>();

        base.SetUpAdditionalDependencies(services);
    }
    
    [Test]
    public async Task CreatePost_PostCreated_GetById()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);

        var post = await Service.CreateUserPost(user1.Id, new UserPostModel
        {
            Id = 0,
            Text = "Test desc",
            CreatedAt = DateTime.Now,
            Files = new List<FileInPostModel>()
            {
                new()
                {
                    Id = 0,
                    FilePath = "testPath"
                }
            },
        });
        
        Assert.That(await Service.GetByIdAsync(post.Id) != null);
        Assert.That(post.UserId == user1.Id);
    }
    
    [Test]
    public async Task CreatePost_DeletePost_CantFind()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);

        var post = await Service.CreateUserPost(user1.Id, new UserPostModel
        {
            Id = 0,
            Text = "Test desc",
            CreatedAt = DateTime.Now,
            Files = new List<FileInPostModel>()
            {
                new()
                {
                    Id = 0,
                    FilePath = "testPath"
                }
            },
        });
        
        Assert.That(await Service.GetByIdAsync(post.Id) != null);
        Assert.That(post.UserId == user1.Id);

        await Service.DeletePost(user1.Id, post.Id);
        
        Assert.ThrowsAsync<PostNotFoundException>(async() => await Service.GetByIdAsync(post.Id));
    }
    
    [Test]
    public async Task CreatePost_EditPost_CantFind()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);

        var post = await Service.CreateUserPost(user1.Id, new UserPostModel
        {
            Id = 0,
            Text = "Test desc",
            CreatedAt = DateTime.Now,
            Files = new List<FileInPostModel>()
            {
                new()
                {
                    FilePath = "testPath"
                }
            },
        });
        
        Assert.That(await Service.GetByIdAsync(post.Id) != null);
        Assert.That(post.UserId == user1.Id);

        var editedPost = await Service.UpdatePost(user1.Id, post.Id, new BasePostModel
        {
            Text = "Updated desc",
            Files = new List<FileInPostModel>
            {
                new FileInPostModel
                {
                    FilePath = "testPath1"
                },
                new FileInPostModel
                {
                    FilePath = "testPath2"
                },
            },
        });
        
        Assert.That(editedPost.Files.Count>post.Files.Count);
    }
    
}