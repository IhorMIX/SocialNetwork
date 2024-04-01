using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IPostService : IBaseService<BasePostModel>
{
    public Task<UserPostModel> CreateUserPost(int userId, BasePostModel post, CancellationToken cancellationToken= default);
    public Task DeletePost(int userId, int postId, CancellationToken cancellationToken= default);
    public Task<UserPostModel> UpdatePost(int userId, int postId, BasePostModel post, CancellationToken cancellationToken= default);
    
    
    
}