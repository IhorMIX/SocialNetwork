using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IMapper _mapper;
    private readonly IChatService _chatService;

    public ChatController(ILogger<ChatController> logger, IMapper mapper, IChatService chatService)
    {
        _logger = logger;
        _mapper = mapper;
        _chatService = chatService;
    }


    [HttpPost("create-group-chat")]
    public async Task<IActionResult> CreateGroupChat([FromBody]ChatCreateViewModel chatCreateViewModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create group chat");
        var userId = User.GetUserId();
        var chat = await _chatService.CreateGroupChat(userId, _mapper.Map<ChatModel>(chatCreateViewModel), cancellationToken);
        _logger.LogInformation("Group chat was created");
        return Ok(_mapper.Map<ChatViewModel>(chat));
    }

    [HttpPost("add-chat-member")]
    public async Task<IActionResult> AddChatMember([FromBody] AddUserInChatModel addUserInChatModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to add user in chat");
        var userId = User.GetUserId();
        await _chatService.AddUsers(userId, addUserInChatModel.chatId, addUserInChatModel.newMeberIds, cancellationToken);
        _logger.LogInformation("User was added in chat");
        return Ok();
    }

    [HttpDelete("del-chat-member")]
    public async Task<IActionResult> DelChatMember([FromQuery] int chatId, [FromQuery] int memberId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete user in chat");
        var userId = User.GetUserId();
        await _chatService.DelMember(userId, chatId, memberId, cancellationToken);
        _logger.LogInformation("User was deleted in chat");
        return Ok();
    }
    
    //EditChat
    [HttpPost("edit-chat")]
    public async Task<IActionResult> EditChat([FromBody] ChatEditModel chatEditModel ,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.EditChat(userId, chatEditModel.ChatId, _mapper.Map<ChatModel>(chatEditModel), cancellationToken);
        return Ok(_mapper.Map<ChatViewModel>(chat));
    }
    
    [HttpDelete("del-chat")]
    public async Task<IActionResult> DelChat([FromQuery] int chatId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete chat");
        var userId = User.GetUserId();
        await _chatService.Delete(userId, chatId, cancellationToken);
        _logger.LogInformation("Chat was deleted");
        return Ok();
    }

    [HttpGet("chats-by-name")]
    public async Task<IActionResult> FindChatByName([FromQuery] string chatName, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.FindChatByName(userId, chatName, cancellationToken);
        return Ok(_mapper.Map<List<ChatViewModel>>(chat));
    }
    
    [HttpGet("all-chats")]
    public async Task<IActionResult> GetAllChats(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.GetAllChats(userId, cancellationToken);
        return Ok(_mapper.Map<List<ChatViewModel>>(chat));
    }
    
    [HttpPost("role")]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleModel createRoleModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to add role");
        var userId = User.GetUserId();
        await _chatService.AddRole(userId, createRoleModel.ChatId, _mapper.Map<RoleModel>(createRoleModel), cancellationToken);
       
        return Ok();
    }
    
    [HttpDelete("role")]
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
    
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles([FromQuery] int chatId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.GetAllChatRoles(userId, chatId, cancellationToken);
        return Ok(_mapper.Map<List<RoleViewModel>>(roles));
    }
    
    [HttpGet("role")]
    public async Task<IActionResult> GetRole([FromQuery] int chatId, [FromQuery] int roleId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.GetRoleById(userId, chatId, roleId, cancellationToken);
        return Ok(_mapper.Map<List<RoleViewModel>>(roles));
    }
    
    [HttpPost("edit-role")]
    public async Task<IActionResult> EditRole([FromBody] RoleUpdateModel roleUpdateModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var role = await _chatService.EditRole(userId, roleUpdateModel.chatId, roleUpdateModel.roleId, 
            _mapper.Map<RoleModel>(roleUpdateModel.rolemodel), cancellationToken);
        return Ok(_mapper.Map<RoleViewModel>(role));
    }

    [HttpGet("chat-members")]
    public async Task<IActionResult> GetChatMembers([FromQuery] int chatId, [FromQuery] int roleId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if(roleId != 0)
            return Ok(_mapper.Map<List<ChatMemberViewModel>>
                (await _chatService.GetChatMembers(userId, chatId, roleId, cancellationToken)));
        
        return Ok(_mapper.Map<List<ChatMemberViewModel>>
            (await _chatService.GetChatMembers(userId, chatId, cancellationToken)));
    }

    [HttpPost("edit-roles-rank")]
    public async Task<IActionResult> EditRolesRank([FromBody] RoleRankUpdateModel roleEditModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _chatService.EditRolesRank(userId, roleEditModel.ChatId, _mapper.Map<List<RoleModel>>(roleEditModel.RoleRanksModel),
            cancellationToken);
        return Ok(_mapper.Map<List<RoleViewModel>>(roles));
    }

}