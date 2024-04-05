using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserPostController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper;
    private readonly IPostService _postService;

    public UserPostController(ILogger<UserController> logger, IMapper mapper, IPostService postService)
    {
        _logger = logger;
        _mapper = mapper;
        _postService = postService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreateUserPostModel post, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Start to creation post");
        var createdPost = await _postService.CreateUserPost(userId, _mapper.Map<UserPostModel>(post), cancellationToken);
        _logger.LogInformation("Post created");
        return Ok(_mapper.Map<PostViewModel>(createdPost));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdatePost([FromBody] CreateUserPostModel post, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Start to creation post");
        var createdPost = await _postService.UpdatePost(userId, post.Id, _mapper.Map<UserPostModel>(post), cancellationToken);
        _logger.LogInformation("Post created");
        return Ok(_mapper.Map<PostViewModel>(createdPost));
    }
    
    
} 