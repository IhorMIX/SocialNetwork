using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class CommentPostRepository : ICommentPostRepository
{

    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public CommentPostRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<CommentPost> GetAll()
    {
        return _socialNetworkDbContext.CommentPost
            .Include(i => i.Post)
            .Include(i => i.User).ThenInclude(r => r.Profile)
            .AsQueryable();
    }

    public async Task<CommentPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.CommentPost
            .Include(i => i.Post)
            .Include(i => i.User).ThenInclude(r => r.Profile)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }


    public async Task<CommentPost> CommentPostAsync(CommentPost commentPost, CancellationToken cancellationToken = default)
    {
        var entireEntity = await _socialNetworkDbContext.CommentPost.AddAsync(commentPost, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return entireEntity.Entity;
    }

    public async Task RemoveCommentAsync(CommentPost commentPost, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.CommentPost.Remove(commentPost);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}