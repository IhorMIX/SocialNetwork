using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Extensions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.BLL.Services.Interfaces;
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
    private readonly IMapper _mapper;

    public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateViewModel user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create user");

        await _userService.CreateUserAsync(_mapper.Map<UserModel>(user), cancellationToken);

        _logger.LogInformation("User was created");

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateViewModel user,
       CancellationToken cancellationToken)
    {
         _logger.LogInformation("Start to update user");

        var userId = User.GetUserId(); //get user id by token
        await _userService.UpdateUserAsync(userId,_mapper.Map<UserModel>(user), cancellationToken); //searching and updating process

        _logger.LogInformation("User was updated");

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("Get user");

        var userId = User.GetUserId(); //get user id by token
        var user = await _userService.GetByIdAsync(userId, cancellationToken); //find user by id
        var viewModel = _mapper.Map<UserViewModel>(user); // put user in userViewModel

        _logger.LogInformation("Get user");

        return Ok(viewModel);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete user");

        await _userService.DeleteUserAsync(User.GetUserId(), cancellationToken); //delete user by id

        _logger.LogInformation("User was deleted");

        return Ok("User was deleted");
    }
    
    [AllowAnonymous]
    [HttpGet("activation/{activationKey}")]
    public async Task<IActionResult> ActivateAccount(string activationKey, CancellationToken cancellationToken)
    {
        var stringId = activationKey.FromBase64ToString();

        if (!int.TryParse(stringId, out var id))
        {
            return BadRequest("Invalid confirmation code");
        }

        await _userService.ActivateUser(id, cancellationToken);

        return Content("<h1>Account activated</h1>", "text/html");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordEmail resetPasswordEmail, CancellationToken cancellationToken)
    {
        await _userService.ResetPasswordConfirmationAsync(resetPasswordEmail.Email, cancellationToken);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel,
        CancellationToken cancellationToken)
    {
        await _userService.ChangePasswordAsync(changePasswordModel.Id, changePasswordModel.NewPassword, cancellationToken);
        return Ok();
    }
}

