using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using System.Data;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleGroupRepository _roleGroupRepository;
    private readonly ILogger<GroupService> _logger;
    private readonly IMapper _mapper;
    private readonly IBannedUserListRepository _bannedUserListRepository;


    public GroupService(
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        ILogger<GroupService> logger,
        IMapper mapper,
        IUserRepository userRepository,
        IRoleGroupRepository roleGroupRepository,
        IBannedUserListRepository bannedUserListRepository)
    {
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _roleGroupRepository = roleGroupRepository;
        _bannedUserListRepository = bannedUserListRepository;
    }

    private async Task<GroupMember?> GetUserInGroupAsync(int userId, int groupId, GroupAccess access,
    CancellationToken cancellationToken)
    {

        var userDb = await _groupMemberRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var isCreator = await IsUserCreatorAsync(userId, groupId, cancellationToken);
        if (isCreator)
        {
            return await _groupMemberRepository.GetAll()
                .Where(c => c.Group.Id == groupId && c.User.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return await _groupMemberRepository.GetAll()
            .Where(c => c.Group.Id == groupId && c.User.Id == userId)
            .SingleOrDefaultAsync(c => c.RoleGroup.Any(r => r.RoleAccesses.Any(i => i.GroupAccess == access)), cancellationToken);
    }

    private async Task<bool> IsUserCreatorAsync(int userId, int groupId, CancellationToken cancellationToken)
    {
        return await _groupMemberRepository.GetAll()
            .AnyAsync(c => c.Group.Id == groupId && c.User.Id == userId && c.IsCreator, cancellationToken);
    }


    public async Task<GroupModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(group, new GroupNotFoundException($"Group with this Id {id} not found"));
        return _mapper.Map<GroupModel>(group);
    }

    public async Task<GroupModel> CreateGroup(int userId, GroupModel groupModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var groupId = await _groupRepository.CreateGroup(_mapper.Map<Group>(groupModel), cancellationToken);
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var member = new GroupMember
        {
            Group = groupDb!,
            User = userDb!,
            IsCreator = true,
        }; await _groupRepository.AddGroupMemberAsync(member, groupDb!, cancellationToken);
        return _mapper.Map<GroupModel>(await _groupRepository.GetByIdAsync(groupId, cancellationToken));
    }

    public async Task DeleteGroup(int userId, int groupId, CancellationToken cancellationToken = default)
    {
        var groupMemberdb = await _groupMemberRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberdb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        if (!groupMemberdb!.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        await _groupRepository.DeleteGroupAsync(groupDb!, cancellationToken);
    }

    public async Task JoinGroup(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));
        if (groupDb == null || userDb == null)
            throw new ArgumentException("Group or user not found");

        var isMember = await _groupRepository.IsUserInGroupAsync(userId, groupDb, cancellationToken);

        if (isMember)
        {
            throw new GroupMemberException($"You cannot to join in group with this ID {groupId} because you are already there");
        }

        var banneduser = await _bannedUserListRepository.GetAll()
            .Where(i => i.UserId == userId)
            .Where(i => i.GroupId == groupId)
            .FirstOrDefaultAsync(cancellationToken);
        if (banneduser != null)
        {
            throw new BannedUserException($"You cannot to join in group with this ID {groupId} because you are banned");
        }

        var member = new GroupMember
        {
            Group = groupDb!,
            User = userDb!,
            IsCreator = false,
        };
        await _groupRepository.AddGroupMemberAsync(member, groupDb!, cancellationToken);



    }

    public async Task LeaveGroup(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new GroupMemberException($"GroupMember with Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with Id {groupId} not found"));

        if (groupMemberDb!.IsCreator)
        {
            throw new CreatorCantLeaveException($"Creator can not leave group");
        }

        await _groupRepository.DelGroupMemberAsync(groupMemberDb.Id, groupDb!, cancellationToken);
    }
    public async Task KickMember(int userId, int groupId, int kickedMemberId, CancellationToken cancellationToken = default)
    {
        if (userId == kickedMemberId)
        {
            throw new GroupMemberException($"You cannnot kick yourself");
        }

        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new GroupMemberException($"GroupMember with Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var kickedmemberDb = await _groupMemberRepository.GetAll()
        .Where(m => m.Group.Id == groupId && m.Id == kickedMemberId).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(kickedmemberDb, new GroupMemberException($"User with Id {kickedMemberId} is not a member of this group"));

        var access = new List<GroupAccess>
        {
            GroupAccess.DelGroupMembers
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        if (kickedmemberDb!.IsCreator || (!groupMemberDb!.IsCreator && kickedmemberDb!.RoleGroup.Select(i => i.Rank).Any(i => i <= groupMemberDb!.RoleGroup.Min(r => r.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }

        await _groupRepository.DelGroupMemberAsync(kickedMemberId, groupDb!, cancellationToken);
        // notific
    }
    public async Task<GroupModel> EditGroup(int userId, int groupId, GroupModel groupModel,
       CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new ChatNotFoundException($"Group with this Id {groupId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.EditGroup
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<GroupModel, Group>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(groupModel);
            var roleTargetValue = roleDbProperty.GetValue(groupDb);

            if (roleSourceValue != null && !ReferenceEquals(roleSourceValue, "") && !roleSourceValue.Equals(roleTargetValue))
            {
                roleDbProperty.SetValue(groupDb, roleSourceValue);
            }
        }

        await _groupRepository.EditGroup(groupDb!, cancellationToken);
        return _mapper.Map<GroupModel>(groupDb);
    }
    public async Task MakeHost(int userId, int groupId, int user2Id, CancellationToken cancellationToken = default)
    {
        var GroupMember1Db = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(GroupMember1Db, new UserNotFoundException($"User with this Id {userId} not found"));
        var GroupMember2Db = await _groupMemberRepository.GetByUserIdAndGroupId(user2Id, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(GroupMember2Db, new UserNotFoundException($"User with this Id {user2Id} not found"));

        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new ChatNotFoundException($"Group with this Id {groupId} not found"));

        if (!GroupMember1Db!.IsCreator)
        {
            throw new NoRightException($"User is not the creator on this group");
        }
        GroupMember1Db!.IsCreator = false;
        await _groupMemberRepository.UpdateGroupMember(GroupMember1Db, cancellationToken);
        GroupMember2Db!.IsCreator = true;
        await _groupMemberRepository.UpdateGroupMember(GroupMember2Db, cancellationToken);

    }
    public async Task AddRole(int userId, int groupId, RoleGroupModel roleGroupModel,
       CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var isAlreadyNamed = await _roleGroupRepository.GetAll()
            .Where(r => r.Group!.Id == groupId && r.RoleName == roleGroupModel.RoleName)
            .FirstOrDefaultAsync(cancellationToken);

        if (isAlreadyNamed is not null)
        {
            _logger.LogInformation("Role with this name is already created");
            throw new Exception("Role with this name is already created");
        }

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var role = _mapper.Map<RoleGroup>(roleGroupModel);
        role.Group = groupDb!;
        if (_roleGroupRepository.GetAll().Count(r => r.Group.Id == groupId) != 0)
            role.Rank = _roleGroupRepository.GetAll().Where(r => r.Group!.Id == groupId).Select(r => r.Rank).Max() + 10;
        _logger.LogInformation("Role was added in group");
        await _roleGroupRepository.CreateRole(role, cancellationToken);
    }
    public async Task DelRole(int userId, int groupId, int roleId, CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));
        var role = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        await _roleGroupRepository.DeleteRole(role!, cancellationToken);
    }
    public async Task<RoleGroupModel> EditRole(int userId, int groupId, int roleId, RoleGroupModel roleGroupModel,
        CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var roleDb = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        if (groupDb!.RoleGroups!.Contains(roleDb!) == false)
            throw new RoleNotFoundException("This role is not in this group");

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }
        if (!groupMemberDb!.IsCreator)
        {
            if (roleDb!.Rank <= groupMemberDb!.RoleGroup.Min(i => i.Rank))
            {
                throw new NoRightException($"You have no rights for it");
            }
        }

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<RoleGroupModel, RoleGroup>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(roleGroupModel);
            var roleTargetValue = roleDbProperty.GetValue(roleDb);

            if (roleSourceValue != null && !ReferenceEquals(roleSourceValue, "") && !roleSourceValue.Equals(roleTargetValue) && roleSourceValue.GetType() == roleTargetValue!.GetType())
            {
                roleDbProperty.SetValue(roleDb, roleSourceValue);
            }

            else if (roleSourceValue is List<GroupAccess> groupAccesses)
            {
                roleDbProperty.SetValue(roleDb, groupAccesses.Select(i => new RoleGroupAccess()
                {
                    GroupAccess = i,
                    RoleId = roleDb!.Id
                }).ToList());
            }

        }
        var existingIds = new HashSet<int>(roleDb!.GroupMembers.Select(r => r.User.Id));
        var newIds = new HashSet<int>(roleGroupModel.UsersIds);

        var idsToAdd = newIds.Except(existingIds);
        var idsToRemove = existingIds.Except(newIds);

        await SetRole(userId, groupId, roleDb.Id, idsToAdd.ToList(), cancellationToken);
        await UnSetRole(userId, groupId, roleDb.Id, idsToRemove.ToList(), cancellationToken);

        roleDb = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));

        await _roleGroupRepository.EditRole(roleDb!, cancellationToken);
        roleDb = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        return _mapper.Map<RoleGroupModel>(roleDb);
    }
    public async Task<RoleGroupModel> GetRoleById(int userId, int groupId, int roleId,
       CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var role = await _roleGroupRepository.GetAll()
            .Where(r => r.Id == roleId && r.Group == groupDb).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));
        return _mapper.Map<RoleGroupModel>(role);
    }
    public async Task SetRole(int userId, int groupId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var roleDb = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var groupMembersDb = await _groupMemberRepository.GetAll().Where(i => i.Group.Id == groupId && userIds.Contains(i.User.Id))
            .ToListAsync(cancellationToken);
        if (groupMembersDb.Count == 0)
            _logger.LogAndThrowErrorIfNull(groupMembersDb, new GroupMemberException($"Group members not found"));
        if (!groupMemberDb!.IsCreator)
        {
            if (groupMembersDb.Any(m => m.RoleGroup.Any(r => r.Rank <= groupMemberDb!.RoleGroup.Min(i => i.Rank))))
            {
                throw new NoRightException($"You have no rights for it");
            }
        }
        if (!groupMemberDb!.IsCreator)
        {
            if (roleDb!.Rank <= groupMemberDb!.RoleGroup.Min(i => i.Rank))
            {
                throw new NoRightException($"You have no rights for it2");
            }
        }


        foreach (var groupMember in groupMembersDb)
        {
            groupMember.RoleGroup.Add(roleDb!);
        }

        await _groupMemberRepository.SetRole(groupMembersDb, cancellationToken);
    }
    public async Task UnSetRole(int userId, int groupId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));
        var roleDb = await _roleGroupRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.EditRoles
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var groupMembersDb = await _groupMemberRepository.GetAll().Where(i => i.Group.Id == groupId && userIds.Contains(i.User.Id))
         .ToListAsync(cancellationToken);

        if (groupMembersDb.Count == 0)
            _logger.LogAndThrowErrorIfNull(groupMembersDb, new GroupMemberException($"Group members not found"));
        if (!groupMemberDb!.IsCreator)
        {
            if (groupMembersDb.Any(m => m.RoleGroup.Any(r => r.Rank <= groupMemberDb!.RoleGroup.Min(i => i.Rank))))
            {
                throw new NoRightException($"You have no rights for it");
            }
        }
        if (!groupMemberDb!.IsCreator)
        {
            if (roleDb!.Rank <= groupMemberDb!.RoleGroup.Min(i => i.Rank))
            {
                throw new NoRightException($"You have no rights for it2");
            }
        }
        foreach (var groupMember in groupMembersDb)
        {
            groupMember.RoleGroup.Remove(roleDb!);
        }

        await _groupMemberRepository.SetRole(groupMembersDb, cancellationToken);
    }

    public async Task BanGroupMember(int userId, int groupId, int memberToBanId, string reason, CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));
        var bannedMemberDb = await _groupMemberRepository.GetByIdAsync(memberToBanId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(bannedMemberDb, new UserNotFoundException($"User with this Id {memberToBanId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.BanGroupMembers
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var kickedmemberDb = await _groupMemberRepository.GetAll()
        .Where(m => m.Group.Id == groupId && m.Id == memberToBanId).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(kickedmemberDb, new GroupMemberException($"User with Id {memberToBanId} is not a member of this group"));

        if (kickedmemberDb!.IsCreator || (!groupMemberDb!.IsCreator && kickedmemberDb!.RoleGroup.Select(i => i.Rank).Any(i => i <= groupMemberDb!.RoleGroup.Min(r => r.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }

        var existingBan = await _bannedUserListRepository.GetAll()
        .Where(b => b.UserId == memberToBanId && b.GroupId == groupId)
        .SingleOrDefaultAsync(cancellationToken);

        if (existingBan != null)
        {
            throw new BannedUserException($"User with ID {memberToBanId} is already banned in group {groupId}");
        }

        await _bannedUserListRepository.BanGroupMemberAsync(bannedMemberDb!.User, groupDb!, reason);
        await KickMember(userId, groupId, memberToBanId);
        // notific
    }

    public async Task UnBanGroupMember(int userId, int groupId, int userToUnBanId, CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var unbannedUserDb = await _userRepository.GetByIdAsync(userToUnBanId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(unbannedUserDb, new UserNotFoundException($"User with this Id {userToUnBanId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.BanGroupMembers
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        await _bannedUserListRepository.UnBanGroupMemberAsync(userToUnBanId, groupId);
    }

    public async Task<PaginationResultModel<BannedUserInGroupModel>> GetAllBannedUser(int userId, int groupId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        var groupMemberDb = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupMemberDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new ChatNotFoundException($"Group with this Id {groupId} not found"));

        var access = new List<GroupAccess>
        {
            GroupAccess.BanGroupMembers
        };
        var hasUserAccess = groupMemberDb!.RoleGroup.HasAccess(access);

        if (!hasUserAccess && !groupMemberDb.IsCreator)
        {
            throw new NoRightException($"You have no rights for it");
        }

        var blackLists = await _bannedUserListRepository.GetAllBannedUserByGroupId(groupDb!.Id)
                                  .Pagination(pagination.CurrentPage, pagination.PageSize)
                                  .ToListAsync(cancellationToken);

        var paginationModel = new PaginationResultModel<BannedUserInGroupModel>
        {
            Data = _mapper.Map<IEnumerable<BannedUserInGroupModel>>(blackLists),
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = blackLists.Count,
        };

        return paginationModel;
    }

    public async Task<PaginationResultModel<RoleGroupModel>> GetAllGroupRoles(int userId, PaginationModel pagination, int groupId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var userInGroup = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInGroup, new GroupMemberException($"User with this Id {userId} not found"));

        var roles = await _roleGroupRepository.GetAll().Where(r => r.Group == groupDb).Pagination(pagination.CurrentPage, pagination.PageSize).ToListAsync(cancellationToken);
        var rolesModels = _mapper.Map<IEnumerable<RoleGroupModel>>(roles);

        var paginationModel = new PaginationResultModel<RoleGroupModel>
        {
            Data = rolesModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = roles.Count(),
        };

        return paginationModel;
    }
    public async Task<PaginationResultModel<GroupModel>> FindGroupByName(int userId, PaginationModel pagination, string groupName,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found")); groupName = groupName.ToLower();
        var groupList = await _groupRepository.GetAll()
            .Where(i => i.GroupMembers!.Any(u => u.User.Id == userId) && i.Name.ToLower().StartsWith(groupName))
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);
        var groupNameModel = _mapper.Map<List<GroupModel>>(groupList);

        var paginationModel = new PaginationResultModel<GroupModel>
        {
            Data = groupNameModel,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = groupList.Count,
        };

        return paginationModel;
    }
    public async Task<PaginationResultModel<GroupModel>> GetAllGroups(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var groupList = new List<Group>();

        groupList = await _groupRepository.GetAll()
            .Where(chat => chat.GroupMembers!.Any(member => member.User.Id == userId))
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var groupNameModel = _mapper.Map<List<GroupModel>>(groupList);

        var paginationModel = new PaginationResultModel<GroupModel>
        {
            Data = groupNameModel,
            CurrentPage = pagination!.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = groupList.Count,
        };

        return paginationModel;
    }

    public async Task<PaginationResultModel<GroupMemberModel>> GetGroupMembers(int userId, PaginationModel pagination, int groupId, int roleGroupId,
       CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));
        var roleDb = await _roleGroupRepository.GetByIdAsync(roleGroupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleGroupId} not found"));

        var userInGroup = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInGroup, new UserNotFoundException($"GroupMember with this Id {userId} not found"));

        var groupMembers = await _groupMemberRepository.GetAll()
            .Where(c => c.RoleGroup.Any(r => r == roleDb) && c.Group == groupDb)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var chatmembersModels = _mapper.Map<IEnumerable<GroupMemberModel>>(groupMembers);

        var paginationModel = new PaginationResultModel<GroupMemberModel>
        {
            Data = chatmembersModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = groupMembers.Count,
        };

        return paginationModel;
    }

    public async Task<PaginationResultModel<GroupMemberModel>> GetGroupMembers(int userId, PaginationModel pagination, int groupId,
       CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var groupDb = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(groupDb, new GroupNotFoundException($"Group with this Id {groupId} not found"));

        var userInGroup = await _groupMemberRepository.GetByUserIdAndGroupId(userId, groupId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInGroup, new UserNotFoundException($"GroupMember with this Id {userId} not found"));

        var groupMembers = await _groupMemberRepository.GetAll()
            .Where(c => c.Group == groupDb)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken); var chatmembersModels = _mapper.Map<IEnumerable<GroupMemberModel>>(groupMembers);

        var paginationModel = new PaginationResultModel<GroupMemberModel>
        {
            Data = chatmembersModels,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = groupMembers.Count,
        };

        return paginationModel;
    }
}