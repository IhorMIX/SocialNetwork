using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class CommentPostService : ICommentPostService
{
    private readonly ICommentPostRepository _commentPostRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ICommentPostService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly INotificationRepository _notificationRepository;
    
    public CommentPostService(ICommentPostRepository commentPostRepository, ILogger<ICommentPostService> logger,
        IPostRepository postRepository, IUserRepository userRepository, IMapper mapper, INotificationRepository notificationRepository)
    {
        _commentPostRepository = commentPostRepository;
        _logger = logger;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _notificationRepository = notificationRepository;
    }

    public async Task<CommentPostModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<CommentPostModel>(await _commentPostRepository.GetByIdAsync(id, cancellationToken));
    }

    /// <returns>Notification ID</returns>
    public async Task<int?> CommentPostAsync(int userId, int postId, string text, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var postDb = await _postRepository.GetByIdAsync(postId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(postDb, new PostNotFoundException($"Post with this Id {postDb} not found"));
        
        var commentDb = await _commentPostRepository.CommentPostAsync(new CommentPost
        {
            PostId = postDb!.Id,
            UserId = userDb!.Id,
            CreatedAt = DateTime.Now,
            Text = text,
            ToReplyCommentId = null,
            ToReplyComment = null
        }, cancellationToken);
        
        if (commentDb.Post is UserPost userPost)
        {
            return await _notificationRepository.CreateNotification(new LikeNotification()
            {
                NotificationMessage =
                    $"{userPost.User.Profile.Name} {userPost.User.Profile.Surname} liked your post {userPost.Files.FirstOrDefault()}",
                CreatedAt = DateTime.Now,
                IsRead = false,
                ToUserId = userPost.UserId,
                InitiatorId = commentDb.UserId,
                LikePostId = commentDb.Id
            }, cancellationToken);
        }
        
        return null;
    }

    public async Task RemoveCommentAsync(int userId, int commentId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var commentDb = await _commentPostRepository.GetByIdAsync(commentId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(commentDb, new CommentNotFoundException($"Comment with this Id {commentId} not found"));

        // check if you admin of group
        if (commentDb!.UserId == userDb!.Id || 
            (commentDb.Post is UserPost post && post.UserId == userDb.Id))
            await _commentPostRepository.RemoveCommentAsync(commentDb!, cancellationToken);
        else
            throw new Exception("You have no rights");
    }

    /// <returns>Notification ID</returns>
    public async Task<int?> ReplyOnCommentAsync(int userId, int commentId, string text,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var commentDb = await _commentPostRepository.GetByIdAsync(commentId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(commentDb, new CommentNotFoundException($"Comment with this Id {commentId} not found"));
        
        var comment = await _commentPostRepository.CommentPostAsync(new CommentPost
        {
            PostId = commentDb!.PostId,
            UserId = userDb!.Id,
            CreatedAt = DateTime.Now,
            Text = text,
            ToReplyCommentId = commentDb.Id
        }, cancellationToken);
        
        if (comment.Post is UserPost userPost)
        {
            return await _notificationRepository.CreateNotification(new LikeNotification()
            {
                NotificationMessage =
                    $"{userPost.User.Profile.Name} {userPost.User.Profile.Surname} liked your post {userPost.Files.FirstOrDefault()}",
                CreatedAt = DateTime.Now,
                IsRead = false,
                ToUserId = userPost.UserId,
                InitiatorId = comment.UserId,
                LikePostId = comment.Id
            }, cancellationToken);
        }
        
        return null;
    }

    public async Task<PaginationResultModel<CommentPostModel>> GetCommentsAsync(int userId, int postId, PaginationModel paginationModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var postDb = await _postRepository.GetByIdAsync(postId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(postDb, new PostNotFoundException($"Post with this Id {postDb} not found"));
        
        var commentsDb = await _commentPostRepository
            .GetAll().Where(r => r.PostId == postDb!.Id)
            .Pagination(paginationModel.CurrentPage, paginationModel.PageSize)
            .ToListAsync(cancellationToken);
        
        return new PaginationResultModel<CommentPostModel>
        {
            Data = _mapper.Map<IEnumerable<CommentPostModel>>(commentsDb),
            CurrentPage = paginationModel!.CurrentPage,
            PageSize = paginationModel.PageSize,
            TotalItems = commentsDb.Count,
        };

    }
}