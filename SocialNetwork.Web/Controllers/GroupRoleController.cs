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
public class GroupRoleController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;

    public GroupRoleController(ILogger<GroupController> logger, IMapper mapper, IGroupService groupService)
    {
        _logger = logger;
        _mapper = mapper;
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleGroupModel createRoleGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to add role");
        var userId = User.GetUserId();
        await _groupService.AddRole(userId, createRoleGroupModel.GroupId, _mapper.Map<RoleGroupModel>(createRoleGroupModel), cancellationToken);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DelRole([FromBody] ForRoleGroupModel forRoleGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete role");
        var userId = User.GetUserId();
        await _groupService.DelRole(userId, forRoleGroupModel.GroupId, forRoleGroupModel.RoleId, cancellationToken);
        _logger.LogInformation("Role was deleted");
        return Ok();
    }

    [HttpPost("set-role")]
    public async Task<IActionResult> SetRole([FromBody] ForRoleGroupModel forRoleGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to set role");
        var userId = User.GetUserId();
        await _groupService.SetRole(userId, forRoleGroupModel.GroupId, forRoleGroupModel.RoleId, forRoleGroupModel.MemberIds!, cancellationToken);
        _logger.LogInformation("The role has been set");
        return Ok();
    }

    [HttpDelete("unset-role")]
    public async Task<IActionResult> UnSetRole([FromBody] ForRoleGroupModel forRoleGroupModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to unsset role");
        var userId = User.GetUserId();
        await _groupService.UnSetRole(userId, forRoleGroupModel.GroupId, forRoleGroupModel.RoleId, forRoleGroupModel.MemberIds!, cancellationToken);
        _logger.LogInformation("The role has been unset");
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> EditRole([FromBody] RoleGroupUpdateModel roleGroupUpdateModel, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var role = await _groupService.EditRole(userId, roleGroupUpdateModel.GroupId, roleGroupUpdateModel.RoleId,
            _mapper.Map<RoleGroupModel>(roleGroupUpdateModel.RoleGroupModel), cancellationToken);
        return Ok(_mapper.Map<RoleGroupViewModel>(role));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllGroupRoles([FromQuery] PaginationModel pagination, [FromQuery] int groupId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var roles = await _groupService.GetAllGroupRoles(userId, pagination, groupId, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<RoleGroupViewModel>>(roles));
    }
}
