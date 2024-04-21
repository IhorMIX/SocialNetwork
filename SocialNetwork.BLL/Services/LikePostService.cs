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

public class LikePostService : ILikePostService
{
    private readonly ILikePostRepository _likePostRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ILikePostService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public LikePostService(ILikePostRepository likePostRepository, IMapper mapper, IUserRepository userRepository,
        ILogger<LikePostService> logger, IPostRepository postRepository)
    {
        _likePostRepository = likePostRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _postRepository = postRepository;
    }

    public async Task<LikePostModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<LikePostModel>(await _likePostRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task<PaginationResultModel<LikePostModel>> GetUsersLikesAsync(int userId, PaginationModel paginationModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        
        var likesDb = await _likePostRepository.GetAll()
            .Pagination(paginationModel.CurrentPage, paginationModel.PageSize).ToListAsync(cancellationToken);
        
        return new PaginationResultModel<LikePostModel>
        {
            Data = _mapper.Map<IEnumerable<LikePostModel>>(likesDb),
            CurrentPage = paginationModel!.CurrentPage,
            PageSize = paginationModel.PageSize,
            TotalItems = likesDb.Count,
        };
    }

    public async Task<PaginationResultModel<LikePostModel>> GetPostLikesAsync(int userId, int postId, PaginationModel paginationModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        
        var postDb = await _postRepository.GetByIdAsync(postId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(postDb, new PostNotFoundException($"Post with this Id {postDb} not found"));
        
        var likesDb = await _likePostRepository.GetAll().Include(r => r.User).ThenInclude(r => r.Profile).Where(r => r.PostId == postDb!.Id)
            .Pagination(paginationModel.CurrentPage, paginationModel.PageSize).ToListAsync(cancellationToken);
        
        return new PaginationResultModel<LikePostModel>
        {
            Data = _mapper.Map<IEnumerable<LikePostModel>>(likesDb),
            CurrentPage = paginationModel!.CurrentPage,
            PageSize = paginationModel.PageSize,
            TotalItems = likesDb.Count,
        };
    }

    public async Task LikePostAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var postDb = await _postRepository.GetByIdAsync(postId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(postDb, new PostNotFoundException($"Post with this Id {postDb} not found"));

        var likePost = new LikePost
        {
            PostId = postDb!.Id,
            UserId = userDb!.Id,
            Post = postDb,
            User = userDb,
            CreatedAt = DateTime.Now
        };

        var like = await _likePostRepository.GetAll()
            .SingleOrDefaultAsync(i => i.PostId == postDb!.Id && i.UserId == userDb!.Id, cancellationToken);

        await (like is null
            ? _likePostRepository.LikePostAsync(likePost, cancellationToken)
            : _likePostRepository.DeleteLikePostAsync(like, cancellationToken));
    }
}