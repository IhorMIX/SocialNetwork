using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class PostRepository : IPostRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    
    public PostRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }
    
    public IQueryable<BasePostEntity> GetAll()
    {
        return _socialNetworkDbContext.Posts.AsQueryable();
    }

    public async Task<BasePostEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Posts.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task CreatePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Posts.AddAsync(post, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Posts.Remove(post);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Posts.Update(post);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}