using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IPostService : IBaseService<BasePostModel>
{
    public Task<BasePostModel> CreateUserPost(int userId, BasePostModel post, CancellationToken cancellationToken = default);
    public Task DeletePost(int creatorId, int postId, CancellationToken cancellationToken = default);
    public Task<BasePostModel> UpdatePost(int userId, int postId, BasePostModel post, CancellationToken cancellationToken = default);
    public Task<PaginationResultModel<UserPostModel>> GetUserPosts(int userId, PaginationModel paginationModel, CancellationToken cancellationToken = default);
    
    
    
}