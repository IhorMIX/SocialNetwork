using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface ILikePostService : IBaseService<LikePostModel>
{
    public Task<PaginationResultModel<LikePostModel>> GetUsersLikesAsync(int userId, PaginationModel paginationModel,
        CancellationToken cancellationToken = default);
    
    public Task<PaginationResultModel<LikePostModel>> GetPostLikesAsync(int userId, int postId, PaginationModel paginationModel,
        CancellationToken cancellationToken = default);
    public Task<int?> LikePostAsync(int userId, int postId, CancellationToken cancellationToken = default);
}