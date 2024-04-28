using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface ICommentPostService : IBaseService<CommentPostModel>
{
    public Task<int?> CommentPostAsync(int userId, int postId, string text, CancellationToken cancellationToken = default);

    public Task RemoveCommentAsync(int userId, int commentId, CancellationToken cancellationToken = default);
    
    public Task<int?> ReplyOnCommentAsync(int userId, int commentId, string text,
        CancellationToken cancellationToken = default);
    
    public Task<PaginationResultModel<CommentPostModel>> GetCommentsAsync(int userId, int postId, PaginationModel paginationModel, CancellationToken cancellationToken = default);
}