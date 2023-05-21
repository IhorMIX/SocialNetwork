using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FriendshipController : ControllerBase
{
    private readonly ILogger<FriendshipController> _logger;
    private readonly TokenHelper _tokenHelper;
    private readonly IMapper _mapper;
    private readonly IFriendshipService _friendshipService;

    public FriendshipController(IFriendshipService friendshipService, TokenHelper tokenHelper, ILogger<FriendshipController> logger, IMapper mapper)
    {
        _friendshipService = friendshipService;
        _tokenHelper = tokenHelper;
        _logger = logger;
        _mapper = mapper;
    }
    
    
    [HttpPost]
    [Route("AddFriend")]
    public async Task<IActionResult> CreateFriendship([FromBody]AddFriendModel model, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendshipService.AddFriendshipAsync(userId, model.Email, cancellationToken);
        return Ok();
    }
    
    [HttpPost]
    [Route("DelFriend")]
    public async Task<IActionResult> DelFriendship([FromBody]AddFriendModel model, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendshipService.DeleteFriendshipAsync(userId, model.Email, cancellationToken);
        return Ok();
    }
}
