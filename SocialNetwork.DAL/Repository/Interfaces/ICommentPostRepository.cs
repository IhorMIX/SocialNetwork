using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface ICommentPostRepository : IBasicRepository<CommentPost>
{
    public Task<CommentPost> CommentPostAsync(CommentPost commentPost, CancellationToken cancellationToken = default);

    public Task RemoveCommentAsync(CommentPost commentPost, CancellationToken cancellationToken = default);
}