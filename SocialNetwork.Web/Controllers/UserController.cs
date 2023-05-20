using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
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

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateViewModel user,
       CancellationToken cancellationToken)
    {
         _logger.LogInformation("Start to update user");

        var userId = User.GetUserId();
        user.Id= userId;
        await _userService.UpdateUserAsync(_mapper.Map<UserModel>(user), cancellationToken);

        _logger.LogInformation("User was updated");

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser([FromBody] ProfileGetViewModel user,
      CancellationToken cancellationToken)
    {
         _logger.LogInformation("Get user");
        //var userId = User.GetUserId();

        await _userService.UpdateUserAsync(_mapper.Map<UserModel>(user), cancellationToken);

        _logger.LogInformation("User was updated");

        return Ok(user);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        //_logger.LogInformation("Start to create user");
        var userId = User.GetUserId();
       // await _userService.DeleteUserAsync(userId, cancellationToken);
        await _userService.DeleteUserAsync(_mapper.Map<UserModel>(userId));

        _logger.LogInformation("User was deleted");

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> AuthorizeUser([FromBody] UserAuthorizeModel model)
    {
        var user = await _userService.GetUserByLoginAndPasswordAsync(model.Login, model.Password);
        var token = _tokenHelper.GetToken(user!.Id);
        
        DateTime? expiredDate = model.IsNeedToRemember ? null : DateTime.Now;
        
        await _userService.AddAuthorizationValueAsync(user, TokenHelper.GenerateRefreshToken(token), 
            LoginType.LocalSystem, expiredDate);

        _logger.LogInformation("User was logined");

        return Ok(token);
    }
   
    [AllowAnonymous]
    [HttpPost("new-token")]
    public async Task<IActionResult> UpdateTokenAsync([FromQuery] string refreshToken)
    {

        refreshToken = refreshToken.Replace(" ", "+"); 
        var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
        var token = _tokenHelper.GetToken(user.Id);
        return Ok(token);
    }

   
    [HttpPost]
    [Route(("logout"))]
    public async Task<IActionResult> LogOutAsync()
    {
        var userId = User.GetUserId();
        await _userService.LogOutAsync(userId);
        return Ok();

    }
}

