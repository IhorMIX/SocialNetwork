using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;

    public GroupController(ILogger<GroupController> logger, IMapper mapper, IGroupService groupService)
    {
        _logger = logger;
        _mapper = mapper;
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] GroupCreateViewModel groupCreateViewModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create group");
        var userId = User.GetUserId();
        var group = await _groupService.CreateGroup(userId, _mapper.Map<GroupModel>(groupCreateViewModel), cancellationToken);
        _logger.LogInformation("Group was created");
        return Ok(_mapper.Map<GroupViewModel>(group));
    }

    [HttpDelete("{groupId:int}")]
    public async Task<IActionResult> DelGroup(int groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete group");
        var userId = User.GetUserId();
        await _groupService.DeleteGroup(userId, groupId, cancellationToken);
        _logger.LogInformation("Group was deleted");
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> EditGroup([FromBody] GroupEditModel groupEditModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _groupService.EditGroup(userId, groupEditModel.GroupId, _mapper.Map<GroupModel>(groupEditModel), cancellationToken);
        return Ok(_mapper.Map<GroupViewModel>(chat));
    }

    [HttpPost("join")]
    public async Task<IActionResult> AddGroupMember([FromBody] AddUserInGroupModel addUserInGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to join user the group");
        var userId = User.GetUserId();
        await _groupService.JoinGroup(addUserInGroupModel.GroupId, userId, cancellationToken);
        _logger.LogInformation("User joined the group");
        return Ok();
    }

    [HttpDelete("leave")]
    public async Task<IActionResult> DelGroupMember([FromBody] AddUserInGroupModel addUserInGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete groupMember from group");
        var userId = User.GetUserId();
        await _groupService.LeaveGroup(addUserInGroupModel.GroupId, userId, cancellationToken);
        _logger.LogInformation("User leave from group");
        return Ok();
    }

    [HttpDelete("group-member")]
    public async Task<IActionResult> DelGroupMember([FromBody] KickGroupMemberModel kickGroupMemberModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete user from group");
        var userId = User.GetUserId();
        await _groupService.KickMember(userId, kickGroupMemberModel.GroupId, kickGroupMemberModel.GroupMemberId, cancellationToken);
        _logger.LogInformation("GroupMember was deleted from group");
        return Ok();
    }

    [HttpPost("make-host")]
    public async Task<IActionResult> MakeHost([FromQuery] int groupId, [FromQuery] int userId, CancellationToken cancellationToken)
    {
        var TokenUserId = User.GetUserId();
        await _groupService.MakeHost(TokenUserId, groupId, userId, cancellationToken);
        return Ok();
    }

    [HttpDelete("ban-member")]
    public async Task<IActionResult> BanMember([FromBody] BanGroupMemberModel banGroupMemberModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to ban member");
        var userId = User.GetUserId();
        await _groupService.BanGroupMember(userId, banGroupMemberModel.GroupId, banGroupMemberModel.BannedGroupMemberId, banGroupMemberModel.Reason, cancellationToken);
        _logger.LogInformation("Member was banned");
        return Ok();
    }

    [HttpPost("unban-member")]
    public async Task<IActionResult> UnBanMember([FromQuery] int groupId, [FromQuery] int userToUnBanId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to unban member");
        var userId = User.GetUserId();
        await _groupService.UnBanGroupMember(userId, groupId, userToUnBanId, cancellationToken);
        _logger.LogInformation("Member was unbanned");
        return Ok();
    }

    [HttpGet("ban-member-by-group")]
    public async Task<IActionResult> GetBannedUsers([FromQuery] int groupId, [FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var loggedInUserId = User.GetUserId();
        var userModels = await _groupService.GetAllBannedUser(loggedInUserId,groupId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BannedUsersInGroupViewModel>>(userModels));
    }

    [HttpGet("groups-by-name")]
    public async Task<IActionResult> FindGroupByName([FromQuery] PaginationModel pagination, [FromQuery] string groupName, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var group = await _groupService.FindGroupByName(userId, pagination, groupName, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<GroupViewModel>>(group));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _groupService.GetAllGroups(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<GroupViewModel>>(chat));
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetGroupMembers([FromQuery] PaginationModel pagination, [FromQuery] int groupId, [FromQuery] int roleGroupId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        if (roleGroupId == 0)
        {
            var output = _mapper.Map<PaginationResultViewModel<GroupMemberViewModel>>
                (await _groupService.GetGroupMembers(userId, pagination, groupId, cancellationToken));
            return Ok(output);
        }
        var outkal = _mapper.Map<PaginationResultViewModel<GroupMemberViewModel>>
            (await _groupService.GetGroupMembers(userId, pagination, groupId, roleGroupId, cancellationToken));
        return Ok(outkal);

    }
}