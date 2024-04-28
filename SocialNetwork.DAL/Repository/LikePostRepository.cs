using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class LikePostRepository : ILikePostRepository
{
    
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public LikePostRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<LikePost> GetAll()
    {
        return _socialNetworkDbContext.LikePost
            .Include(i => i.Post)
            .Include(i => i.User)
            .AsQueryable();
    }

    public async Task<LikePost?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.LikePost
            .Include(i => i.Post)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<LikePost> LikePostAsync(LikePost likePost, CancellationToken cancellationToken = default)
    {
        var entityEntry = await _socialNetworkDbContext.LikePost.AddAsync(likePost, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity;
    }

    public async Task DeleteLikePostAsync(LikePost likePost, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.LikePost.Remove(likePost);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}