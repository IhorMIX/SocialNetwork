using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipService _friendshipService;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<ChatService> _logger;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IMapper _mapper;
    private readonly RoleOption _roleOptions;
    private readonly INotificationRepository _notificationRepository;

    public ChatService(
        IChatRepository chatRepository,
        ILogger<ChatService> logger,
        IMapper mapper,
        IUserRepository userRepository,
        IFriendshipService friendshipService,
        IRoleRepository roleRepository,
        IChatMemberRepository chatMemberRepository,
        IOptions<RoleOption> roleOptions, INotificationRepository notificationRepository)
    {
        _chatRepository = chatRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _friendshipService = friendshipService;
        _roleRepository = roleRepository;
        _chatMemberRepository = chatMemberRepository;
        _notificationRepository = notificationRepository;
        _roleOptions = roleOptions.Value;
    }

    private Task<ChatMember?> GetUserInChatAsync(int userId, int chatId, ChatAccess access,
        CancellationToken cancellationToken)
    {
        return _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User.Id == userId)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.RoleAccesses.Any(i => i.ChatAccess == access)), cancellationToken);
    }

    public async Task<ChatModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chat, new ChatNotFoundException($"Chat with this Id {id} not found"));
        return _mapper.Map<ChatModel>(chat);
    }

    public async Task CreateP2PChat(int userId, int user2Id, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        _logger.LogAndThrowErrorIfNull(user2Db, new UserNotFoundException($"User with this Id {userId} not found"));
        if (await _friendshipService.IsFriends(userDb!.Id, user2Db!.Id, cancellationToken) is false)
            return;

        chatModel.IsGroup = false;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        //p2pAdmin role
        var roleList = new List<Role>()
            { (await _roleRepository.GetByIdAsync(_roleOptions.RoleP2PAdminId, cancellationToken))! };

        await _chatRepository.AddChatMemberAsync(new ChatMember
        {
            Chat = chatDb!,
            User = userDb,
            Role = new List<Role>(roleList)
        }, chatDb!, cancellationToken);
        await _chatRepository.AddChatMemberAsync(new ChatMember
        {
            Chat = chatDb!,
            User = user2Db,
            Role = new List<Role>(roleList)
        }, chatDb!, cancellationToken);
    }

    public async Task<ChatModel> CreateGroupChat(int userId, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        chatModel.IsGroup = true;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var roleDb = await _roleRepository.CreateRole(new Role
        {
            RoleName = "everyone",
            RoleColor = "#FFFFFF",
            RoleAccesses = new List<RoleChatAccess>()
            {
                new () { ChatAccess =  ChatAccess.SendMessages },
                new () { ChatAccess =  ChatAccess.SendAudioMess },
                new () { ChatAccess =  ChatAccess.SendFiles },
                new () { ChatAccess =  ChatAccess.DelMessages },
            },
            Chat = chatDb,
            Rank = 100000
        }, cancellationToken);
        
        //admin role
        var roleList = new List<Role>
        {
            (await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken))!,
            roleDb
        };

        var member = new ChatMember
        {
            Chat = chatDb!,
            User = userDb!,
            Role = new List<Role>(roleList)
        };
        await _chatRepository.AddChatMemberAsync(member, chatDb!, cancellationToken);
        return _mapper.Map<ChatModel>(await _chatRepository.GetByIdAsync(chatId, cancellationToken));
    }

    public async Task AddUsers(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        if (chatDb!.IsGroup is false)
        {
            _logger.LogAndThrowErrorIfNull(chatDb, new NoRightException($"Chat is not group"));
        }
    
        var usersDb = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var notFoundUsers = userIds.Where(u => !usersDb.Select(i => i.Id).Contains(u)).ToList();
        if (notFoundUsers.Any())
        {
            throw new UserNotFoundException($"Users with ids {string.Join(", ", notFoundUsers)} not found");
        }
        
        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.AddMembers, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));
    
        var alreadyIn = await _chatMemberRepository.GetAll()
            .Where(c => userIds.Contains(c.User.Id) && c.Chat.Id == chatId).Select(u => u.User.Id)
            .ToListAsync(cancellationToken);
        var idsToAdd = userIds.Except(alreadyIn);
    
        var roleList = new List<Role>()
        {
            (await _roleRepository.GetAll().Where(r => r.Chat == chatDb)
                .FirstOrDefaultAsync(r => r.RoleName == "everyone", cancellationToken))!
        };
        var usersToAdd = await _userRepository.GetAll().Where(i => idsToAdd.Contains(i.Id))
            .ToListAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(usersToAdd, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
    
        List<ChatMember> chatMembers = new List<ChatMember>();
        foreach (var userToAdd in usersToAdd)
        {
            chatMembers.Add(new ChatMember
            {
                Chat = chatDb,
                User = userToAdd,
                Role = new List<Role>(roleList)
            });
        }
    
        await _chatRepository.AddChatMemberAsync(chatMembers, chatDb, cancellationToken);
        
        // in notification box
        var chatNotifications = usersToAdd.Select(userToAdd => new ChatNotification
        {
            Description = "You have been added to the chat",
            CreatedAt = DateTime.Now,
            IsRead = false,
            UserId = userToAdd.Id,
            ChatId = chatDb.Id,
            ChatName = chatDb.Name,
            Logo = chatDb.Logo
        }).Cast<BaseNotificationEntity>().ToList();
        
        await _notificationRepository.CreateNotifications(chatNotifications, cancellationToken);
        
    }
    
    public async Task DelMembers(int userId, int chatId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.DelMembers, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var membersToDel = await _chatMemberRepository.GetAll().Where(i => i.Chat.Id == chatId && userIds.Contains(i.User.Id))
            .ToListAsync(cancellationToken);
        if(membersToDel.Count == 0)
            _logger.LogAndThrowErrorIfNull(userInChat, new ChatMemberException($"Chat members not found"));

        if (membersToDel.Any(m => m.Role.Any(r => r.Rank <= userInChat!.Role.Min(i => i.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var adminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);

        if (membersToDel.SingleOrDefault(i => i.Role.Contains(_mapper.Map<Role>(adminRole))) is not null ||
            membersToDel.Contains(userInChat!))
            _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));
            
        await _chatRepository.DelChatMemberAsync(membersToDel, chatDb!, cancellationToken);
        
        // notification box
        var chatNotifications = membersToDel.Select(userToDel => new ChatNotification
        {
            Description = $"You have been kicked out of the chat '{chatDb!.Name}'",
            CreatedAt = DateTime.Now,
            IsRead = false,
            UserId = userToDel.User.Id,
            ChatId = chatDb.Id,
            ChatName = chatDb.Name,
            Logo = chatDb.Logo
        }).Cast<BaseNotificationEntity>().ToList();
        await _notificationRepository.CreateNotifications(chatNotifications, cancellationToken);
        
    }

    public async Task DeleteChat(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.Id == _roleOptions.RoleAdminId), cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        await _chatRepository.DeleteChatAsync(chatDb!, cancellationToken);
    }

    public async Task<ChatModel> EditChat(int userId, int chatId, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));


        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditChat, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<ChatModel, Chat>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(chatModel);
            var roleTargetValue = roleDbProperty.GetValue(chatDb);

            if (roleSourceValue != null && !ReferenceEquals(roleSourceValue, "") && !roleSourceValue.Equals(roleTargetValue))
            {
                roleDbProperty.SetValue(chatDb, roleSourceValue);
            }
        }
        await _chatRepository.EditChat(chatDb!, cancellationToken);

        // in chat
        // user edited chat (and enumerate changes)
        //
        
        return _mapper.Map<ChatModel>(chatDb);
    }

    public async Task<List<ChatModel>> FindChatByName(int userId, string chatName,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        chatName = chatName.ToLower();
        var chatList = await _chatRepository.GetAll()
            .Where(i => i.ChatMembers!.Any(u => u.User.Id == userId) && i.Name.ToLower().StartsWith(chatName))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task<List<ChatModel>> GetAllChats(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var chatList = await _chatRepository.GetAll()
            .Where(chat => chat.ChatMembers!.Any(member => member.User.Id == userId))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task AddRole(int userId, int chatId, RoleModel roleModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var isAlreadyNamed = await _roleRepository.GetAll()
            .Where(r => r.Chat!.Id == chatId && r.RoleName == roleModel.RoleName)
            .FirstOrDefaultAsync(cancellationToken);
        if (isAlreadyNamed is not null)
        {
            _logger.LogInformation("Role with this name is already created");
            throw new Exception("Role with this name is already created");
        }

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = _mapper.Map<Role>(roleModel);
        role.Chat = chatDb;
        role.Rank = _roleRepository.GetAll().Where(r => r.Chat!.Id == chatId).Select(r => r.Rank).Max() + 10;
        _logger.LogInformation("Role was added in chat");
        await _roleRepository.CreateRole(role, cancellationToken);
    }

    public async Task<RoleModel> GetRoleById(int userId, int chatId, int roleId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = await _roleRepository.GetAll()
            .Where(r => r.Id == roleId && r.Chat == chatDb).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));
        return _mapper.Map<RoleModel>(role);
    }

    public async Task DelRole(int userId, int chatId, int roleId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));
        await _roleRepository.DeleteRole(role!, cancellationToken);
    }


    public async Task<RoleModel> EditRole(int userId, int chatId, int roleId, RoleModel roleModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));

        if (chatDb!.Roles!.Contains(roleDb!) == false)
            throw new Exception("This role is not in this chat");

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        if (roleDb!.Rank <= userInChat!.Role.Min(i => i.Rank))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        if (_chatMemberRepository.GetAll().Where(i => i.User.Id != userId)
            .Where(i => i.Role.Any(r => r.Id == roleId)).ToList()
            .Any(m => m.Role.Any(r => r.Rank <= userInChat.Role.Min(i => i.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<RoleModel, Role>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(roleModel);
            var roleTargetValue = roleDbProperty.GetValue(roleDb);

            if (roleSourceValue != null && !ReferenceEquals(roleSourceValue, "") && !roleSourceValue.Equals(roleTargetValue) && roleSourceValue.GetType() == roleTargetValue!.GetType())
            {
                roleDbProperty.SetValue(roleDb, roleSourceValue);
            }

            else if (roleSourceValue is List<ChatAccess> chatAccesses && chatAccesses.Any())
            {
                roleDbProperty.SetValue(roleDb, chatAccesses.Select(i => new RoleChatAccess()
                {
                    ChatAccess = i,
                    RoleId = roleDb.Id
                }).ToList());
            }
        }

        var existingIds = new HashSet<int>(roleDb.ChatMembers.Select(r => r.User.Id));
        var newIds = new HashSet<int>(roleModel.UsersIds);

        var idsToAdd = newIds.Except(existingIds);
        var idsToRemove = existingIds.Except(newIds);

        await SetRole(userId, chatId, roleDb.Id, idsToAdd.ToList(), cancellationToken);
        await UnSetRole(userId, chatId, roleDb.Id, idsToRemove.ToList(), cancellationToken);

        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));

        await _roleRepository.EditRole(roleDb!, cancellationToken);
        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        return _mapper.Map<RoleModel>(roleDb);
    }


    public async Task SetRole(int userId, int chatId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));
        
        var chatMembersDb = await _chatMemberRepository.GetAll().Where(i => i.Chat.Id == chatId && userIds.Contains(i.User.Id))
            .ToListAsync(cancellationToken);
        if(chatMembersDb.Count == 0)
            _logger.LogAndThrowErrorIfNull(userInChat, new ChatMemberException($"Chat members not found"));
        if (chatMembersDb.Any(m => m.Role.Any(r => r.Rank <= userInChat!.Role.Min(i => i.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        if (roleDb!.Rank <= userInChat!.Role.Min(i => i.Rank))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        foreach (var chatMember in chatMembersDb)
        {
            chatMember.Role.Add(roleDb);
        }
      
        await _chatMemberRepository.SetRole(chatMembersDb, cancellationToken);
    }

    public async Task UnSetRole(int userId, int chatId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));
        
        var chatMembersDb = await _chatMemberRepository.GetAll().Where(i => i.Chat.Id == chatId && userIds.Contains(i.User.Id))
            .ToListAsync(cancellationToken);
        if(chatMembersDb.Count == 0)
            _logger.LogAndThrowErrorIfNull(userInChat, new ChatMemberException($"Chat members not found"));
        if (chatMembersDb.Any(m => m.Role.Any(r => r.Rank <= userInChat!.Role.Min(i => i.Rank))))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        foreach (var chatMember in chatMembersDb)
        {
            chatMember.Role.Remove(roleDb!);
        }
        
        await _chatMemberRepository.SetRole(chatMembersDb, cancellationToken);
    }

    public async Task<List<RoleModel>> GetAllChatRoles(int userId, int chatId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var roles = await _roleRepository.GetAll().Where(r => r.Chat == chatDb).ToListAsync(cancellationToken);
        return _mapper.Map<List<RoleModel>>(roles);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, int roleId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Role.Any(r => r == roleDb) && c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }

    public async Task<List<RoleModel>> EditRolesRank(int userId, int chatId, List<RoleModel> roleModels,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));
        
        var roleIds = roleModels.Select(rm => rm.Id).ToList();
        var rolesDb = await _roleRepository.GetAll().Where(r => r.Chat!.Id == chatDb!.Id && roleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (rolesDb.Any(r => r.Rank <= userInChat!.Role.Min(i => i.Rank)))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        for (int i = 0; i < roleModels.Count; i++)
        {
            rolesDb[i].Rank = roleModels[i].Rank;
        }

        await _roleRepository.EditRole(rolesDb, cancellationToken);
        return _mapper.Map<List<RoleModel>>(rolesDb);
    }

    public async Task LeaveChat(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var adminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);
        if (userInChat!.Role.Contains(_mapper.Map<Role>(adminRole)))
        {
            throw new CreatorCantLeaveException("You are creator, you can`t do this");
        }
        
        await _chatRepository.DelChatMemberAsync(new List<ChatMember>{userInChat}, chatDb!, cancellationToken);
        
        // in chat
        // user leaved
    }

    public async Task MakeHost(int userId, int chatId, int user2Id, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var adminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);
        if (!userInChat!.Role.Contains(_mapper.Map<Role>(adminRole)))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var user2InChat = await _chatMemberRepository.GetByUserIdAndChatId(user2Id, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(user2InChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        adminRole!.ChatMembers.Remove(userInChat);
        adminRole.ChatMembers.Add(user2InChat!);
        await _roleRepository.EditRole(_mapper.Map<Role>(adminRole), cancellationToken);
        
        // in chat
        // user is new host
    }
}