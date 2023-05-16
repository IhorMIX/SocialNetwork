using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    private readonly IUserService _userService;
    private readonly TokenHelper _tokenHelper;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, TokenHelper tokenHelper, ILogger<UserController> logger, IMapper mapper)
    {
        _userService = userService;
        _tokenHelper = tokenHelper;
        _logger = logger;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateViewModel user,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create user");
        
        await _userService.CreateUserAsync(_mapper.Map<UserModel>(user), cancellationToken);

        _logger.LogInformation("User was created");

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> AuthorizeUser([FromBody] UserAuthorizeModel model)
    {
        var user = await _userService.GetUserByLoginAndPasswordAsync(model.Login, model.Password);
        var token = _tokenHelper.GetToken(user!.Id);
        await _userService.AddAuthorizationValueAsync(user, TokenHelper.GenerateRefreshToken(token), LoginType.LocalSystem);
        return Ok(token);
    }
   
    
}