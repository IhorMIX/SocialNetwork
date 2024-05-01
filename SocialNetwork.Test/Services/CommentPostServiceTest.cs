using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;



public class CommentPostServiceTest : DefaultServiceTest<ICommentPostService, CommentPostService>
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
        services.AddScoped<ICommentPostRepository, CommentPostRepository>();
        
        base.SetUpAdditionalDependencies(services);
    }

    [Test]
    public async Task CreatePost_AddComment_Ok()
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

        string comment = "test_Comment";
        await Service.CommentPostAsync(user2.Id, post.Id, comment);

        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 1);
        Assert.That(post!.Comments.SingleOrDefault(r => r.Text == comment && r.UserId == user2.Id) != null);
        
        comment = "test_Comment 2";
        await Service.CommentPostAsync(user1.Id, post.Id, comment);

        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 2);
        Assert.That(post!.Comments.SingleOrDefault(r => r.Text == comment && r.UserId == user1.Id) != null);
        
    }
    
    [Test]
    public async Task AddComment_ReplyOnComment_Ok()
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

        string comment = "test_Comment";
        await Service.CommentPostAsync(user2.Id, post.Id, comment);

        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 1);
        Assert.That(post!.Comments.SingleOrDefault(r => r.Text == comment && r.UserId == user2.Id) != null);

        var commentId = post.Comments.Where(r => r.Text == comment).Select(r => r.Id).SingleOrDefault();
        comment = "reply on comment";
        await Service.ReplyOnCommentAsync(user1.Id, commentId, comment);

        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 2);
        Assert.That(post!.Comments
            .SingleOrDefault(r => r.Text == comment && 
                                  r.UserId == user1.Id && 
                                  r.ToReplyCommentId == commentId) != null);
    }
    
    [Test]
    public async Task AddComment_Remove_Ok()
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

        string comment = "test_Comment";
        var commentDb = await Service.CommentPostAsync(user2.Id, post.Id, comment);

        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 1);
        Assert.That(post!.Comments.SingleOrDefault(r => r.Text == comment && r.UserId == user2.Id) != null);
        
        var commentId = post.Comments.Where(r => r.Text == comment).Select(r => r.Id).SingleOrDefault();
        
        await Service.RemoveCommentAsync(user1.Id, commentId);
        post = await postService.GetByIdAsync(post.Id);
        
        Assert.That(post!.Comments.Count == 0);
    }
}