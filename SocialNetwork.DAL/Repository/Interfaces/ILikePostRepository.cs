using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface ILikePostRepository : IBasicRepository<LikePost>
{
    public Task<LikePost> LikePostAsync(LikePost likePost, CancellationToken cancellationToken = default);
    public Task DeleteLikePostAsync(LikePost likePost, CancellationToken cancellationToken = default);
}