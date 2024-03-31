using AutoMapper;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BasePostModel> _logger;


    public PostService(IPostRepository postRepository, IUserRepository userRepository, IMapper mapper, ILogger<BasePostModel> logger)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BasePostModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(post,
            new PostNotFoundException($"Post with id {id} not found"));
        return _mapper.Map<BasePostModel>(post);
    }

    public async Task<UserPostModel> CreateUserPost(int userId, BasePostModel post, CancellationToken cancellationToken)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var userPost = new UserPostModel()
        {
            Text = post.Text,
            CreatedAt = post.CreatedAt,
            Files = post.Files,
            UserId = userDb!.Id,
        };

        await _postRepository.CreatePost(_mapper.Map<UserPost>(userPost), cancellationToken);

        return userPost;
    }

    public async Task DeletePost(int userId, int postId, CancellationToken cancellationToken)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(post, new PostNotFoundException($"Post with id {postId} not found"));

        await _postRepository.DeletePost(post!, cancellationToken);
    }

    public Task<UserPostModel> UpdatePost(int userId, int postId, BasePostModel post, CancellationToken cancellationToken)
    {
        return null;
    }
}