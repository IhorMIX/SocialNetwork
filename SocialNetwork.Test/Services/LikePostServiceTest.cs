using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class LikePostServiceTest : DefaultServiceTest<ILikePostService, LikePostService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();

        services.AddScoped<IRequestRepository, RequestRepository>();
            
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ILikePostRepository, LikePostRepository>();
        
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreatePost_LikeIt_Ok()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
    
        var postService = ServiceProvider.GetRequiredService<IPostService>();
        var post = await postService.CreateUserPost(user1.Id, new UserPostModel
        {
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
        
        Assert.That(await postService.GetByIdAsync(post.Id) != null);
        Assert.That((post as UserPostModel)!.UserId == user1.Id);
        Assert.That(post.Likes.Count == 0);
        
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        await Service.LikePostAsync(user2.Id, post.Id);
        await Service.LikePostAsync(user1.Id, post.Id);
        
        post = await postService.GetByIdAsync(post.Id);
        user2 = await userService.GetByIdAsync(user2.Id);
        user1 = await userService.GetByIdAsync(user1.Id);
        
        Assert.That(post!.Likes.Count == 2);
        Assert.That(user1!.LikedPosts!.Count == 1);
        Assert.That(user2!.LikedPosts!.Count == 1);
        Assert.That(post!.Likes.SingleOrDefault(r => r.UserId == user1.Id) != null);
        Assert.That(post!.Likes.SingleOrDefault(r => r.UserId == user2.Id) != null);
        
        
    }
    
    [Test]
    public async Task CreatePost_LikeIt2Times_NoLikes()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
    
        var postService = ServiceProvider.GetRequiredService<IPostService>();
        var post = await postService.CreateUserPost(user1.Id, new UserPostModel
        {
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
        
        Assert.That(await postService.GetByIdAsync(post.Id) != null);
        Assert.That((post as UserPostModel)!.UserId == user1.Id);
        Assert.That(post.Likes.Count == 0);
        
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        
        await Service.LikePostAsync(user2.Id, post.Id);
        
        post = await postService.GetByIdAsync(post.Id);
        user2 = await userService.GetByIdAsync(user2.Id);
        Assert.That(post!.Likes.Count == 1);
        Assert.That(user2!.LikedPosts!.Count == 1);
        Assert.That(post!.Likes.SingleOrDefault(r => r.UserId == user2.Id) != null);
        
        await Service.LikePostAsync(user2.Id, post.Id);
        
        post = await postService.GetByIdAsync(post.Id);
        user2 = await userService.GetByIdAsync(user2.Id);
        Assert.That(post!.Likes.Count == 0);
        Assert.That(user2!.LikedPosts!.Count == 0);
        Assert.That(post!.Likes.SingleOrDefault(r => r.UserId == user2.Id) == null);
    }    
}