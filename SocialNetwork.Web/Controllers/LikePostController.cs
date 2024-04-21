using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikePostController : ControllerBase
{
    private readonly ILogger<LikePostController> _logger;
    private readonly IMapper _mapper;
    private readonly ILikePostService _likePostService;

    public LikePostController(ILogger<LikePostController> logger, IMapper mapper, ILikePostService likePostService)
    {
        _logger = logger;
        _mapper = mapper;
        _likePostService = likePostService;
    }

    [HttpPost("{postId:int}")]
    public async Task<IActionResult> LikePost(int postId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _likePostService.LikePostAsync(userId, postId, cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetLikes([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var likes = await _likePostService.GetUsersLikesAsync(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<LikePostViewModel>>(likes));
    }
    
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetPostLikes(int postId, [FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var likes = await _likePostService.GetPostLikesAsync(userId, postId,pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<LikePostViewModel>>(likes));
    }
    
}