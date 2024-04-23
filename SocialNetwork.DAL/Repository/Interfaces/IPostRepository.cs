using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IPostRepository : IBasicRepository<BasePostEntity>
{
    public Task<BasePostEntity> CreatePost(BasePostEntity post, CancellationToken cancellationToken = default);

    public Task DeletePost(BasePostEntity post, CancellationToken cancellationToken = default);

    public Task<BasePostEntity> UpdatePost(BasePostEntity post, CancellationToken cancellationToken = default);
}