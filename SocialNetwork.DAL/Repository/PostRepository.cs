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
        return _socialNetworkDbContext.Posts
            .Include(r => r.Files)
            .Include(r => r.Comments)
            .Include(r => r.Likes)
            .Include(r => (r as UserPost)!.User).ThenInclude(r => r.Profile)
            //.Include(r => (r as GroupPost)!.Group)
            .AsQueryable();
    }

    public async Task<BasePostEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Posts
            .Include(r => r.Files)
            .Include(r => r.Comments)
            .Include(r => r.Likes)
            .Include(r => (r as UserPost)!.User).ThenInclude(r => r.Profile)
            //.Include(r => (r as GroupPost)!.Group)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<BasePostEntity> CreatePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        var postDb = await _socialNetworkDbContext.Posts.AddAsync(post, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return postDb.Entity;
    }

    public async Task DeletePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Posts.Remove(post);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<BasePostEntity> UpdatePost(BasePostEntity post, CancellationToken cancellationToken = default)
    {
        var postDb = _socialNetworkDbContext.Posts.Update(post);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return postDb.Entity;
    }
}