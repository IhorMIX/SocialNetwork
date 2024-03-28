using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IPostRepository : IBasicRepository<BasePostEntity>
{
    public Task CreatePost(BasePostEntity post, CancellationToken cancellationToken = default);

    public Task DeletePost(BasePostEntity post, CancellationToken cancellationToken = default);

    public Task UpdatePost(BasePostEntity post, CancellationToken cancellationToken = default);
}