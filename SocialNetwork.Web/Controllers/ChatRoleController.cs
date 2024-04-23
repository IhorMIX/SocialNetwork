using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatRoleController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IMapper _mapper;
    private readonly IChatService _chatService;

    public ChatRoleController(ILogger<ChatController> logger, IMapper mapper, IChatService chatService)
    {
        _logger = logger;
        _mapper = mapper;
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleModel createRoleModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to add role");
        var userId = User.GetUserId();
        await _chatService.AddRole(userId, createRoleModel.ChatId, _mapper.Map<RoleModel>(createRoleModel), cancellationToken);
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DelRole([FromBody] IdsForRoleModel idsForRoleModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete role");
        var userId = User.GetUserId();
        await _chatService.DelRole(userId, idsForRoleModel.ChatId, idsForRoleModel.RoleId, cancellationToken);
        _logger.LogInformation("Role was deleted");
        return Ok();
    }
    
    [HttpPost("set-role")]
    public async Task<IActionResult> SetRole([FromBody] IdsForRoleModel idsForRoleModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to set role");
        var userId = User.GetUserId();
        await _chatService.SetRole(userId, idsForRoleModel.ChatId, idsForRoleModel.RoleId, idsForRoleModel.MemberIds!, cancellationToken);
        _logger.LogInformation("Role was set");
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllRoles([FromQuery] PaginationModel pagination, [FromQuery] int chatId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.GetAllChatRoles(userId, pagination, chatId, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<RoleViewModel>>(roles));
    }
    
    [HttpGet("role")]
    public async Task<IActionResult> GetRole([FromQuery] int chatId, [FromQuery] int roleId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.GetRoleById(userId, chatId, roleId, cancellationToken);
        return Ok(_mapper.Map<List<RoleViewModel>>(roles));
    }
    
    [HttpPut("edit-role")]
    public async Task<IActionResult> EditRole([FromBody] RoleUpdateModel roleUpdateModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var role = await _chatService.EditRole(userId, roleUpdateModel.ChatId, roleUpdateModel.RoleId, 
            _mapper.Map<RoleModel>(roleUpdateModel.RoleModel), cancellationToken);
        return Ok(_mapper.Map<RoleViewModel>(role));
    }
    
    
    [HttpPut("edit-roles-rank")]
    public async Task<IActionResult> EditRolesRank([FromBody] RoleRankUpdateModel roleEditModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.EditRolesRank(userId, roleEditModel.ChatId, _mapper.Map<List<RoleModel>>(roleEditModel.RoleRanksModel),
            cancellationToken);
        return Ok(_mapper.Map<List<RoleViewModel>>(roles));
    }
    
}