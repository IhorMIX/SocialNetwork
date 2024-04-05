using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<UserPostModel> CreateUserPost(int userId, BasePostModel post, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var userPost = new UserPostModel()
        {
            Text = post.Text,
            CreatedAt = DateTime.Now,
            Files = post.Files,
            UserId = userDb!.Id,
        };

        var postDb = await _postRepository.CreatePost(_mapper.Map<UserPost>(userPost), cancellationToken);

        return _mapper.Map<UserPostModel>(postDb);
    }

    public async Task DeletePost(int userId, int postId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var post = await _postRepository.GetAll().Where(r => r.Id == postId && (r as UserPost)!.UserId == userId).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(post, new PostNotFoundException($"Post with id {postId} not found"));
        
        await _postRepository.DeletePost(post!, cancellationToken);
    }

    public async Task<UserPostModel> UpdatePost(int userId, int postId, BasePostModel post, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        
        var postDb = await _postRepository.GetAll().Where(r => r.Id == postId).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(postDb, new PostNotFoundException($"Post with id {postId} not found"));
        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<BasePostModel, BasePostEntity>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(post);
            var roleTargetValue = roleDbProperty.GetValue(postDb);
            
            if (roleSourceValue != null 
                && roleSourceValue!.GetType()!=typeof(DateTime) 
                && !roleSourceValue.Equals(0) 
                && !ReferenceEquals(roleSourceValue, "") 
                && !roleSourceValue.Equals(roleTargetValue))
            {
                if (roleSourceValue!.GetType() == typeof(List<FileInPostModel>))
                {
                    postDb!.Files = _mapper.Map<List<FileInPost>>(roleSourceValue);
                }
                else
                { 
                    roleDbProperty.SetValue(postDb, roleSourceValue);
                }
            }
        }
        
        return _mapper.Map<UserPostModel>(await _postRepository.UpdatePost(postDb!, cancellationToken));
    }
}