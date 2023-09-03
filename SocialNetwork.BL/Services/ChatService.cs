using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipService _friendshipService;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<ChatService> _logger;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IMapper _mapper;
    //private readonly IConfiguration _configuration;
    
    public ChatService(
        IChatRepository chatRepository, 
        ILogger<ChatService> logger, 
        IMapper mapper, 
        IUserRepository userRepository, 
        IFriendshipService friendshipService, 
        IRoleRepository roleRepository, 
        IChatMemberRepository chatMemberRepository)
    {
        _chatRepository = chatRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _friendshipService = friendshipService;
        _roleRepository = roleRepository;
        _chatMemberRepository = chatMemberRepository;
        //_configuration = configuration;
    }

    public async Task<ChatModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetByIdAsync(id, cancellationToken);
        if (chat is null)
        {
            _logger.LogError("Chat with this Id {Id} not found", id);
            throw new Exception($"Chat with Id '{id}' not found");
        }
        
        return _mapper.Map<ChatModel>(chat);
    }

    public async Task CreateP2PChat(int userId, int user2Id, ChatModel chatModel, CancellationToken cancellationToken = default)
    {

        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Id, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        _logger.IsExists(user2Db, new UserNotFoundException($"User with this Id {userId} not found"));
        if (await _friendshipService.IsFriends(userDb!.Id, user2Db!.Id, cancellationToken) is false) 
            return;
        
        chatModel.isGroup = false;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        //p2pAdmin role
        var roleList = new List<Role>() { (await _roleRepository.GetByIdAsync(3, cancellationToken))! };
        //roleList.Add((await _roleRepository.GetByIdAsync(_configuration.GetValue<int>("RoleP2PAdminId"), cancellationToken))!);
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
    
    public async Task<ChatModel> CreateGroupChat(int userId, ChatModel chatModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        
        chatModel.isGroup = true;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        //admin role
        var roleList = new List<Role>() { (await _roleRepository.GetByIdAsync(2, cancellationToken))! };
        //roleList.Add((await _roleRepository.GetByIdAsync(_configuration.GetValue<int>("RoleAdminId"), cancellationToken))!);
        roleList.Add((await _roleRepository.GetByIdAsync(1, cancellationToken))!);
        
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
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        if (chatDb!.IsGroup is false)
        {
            _logger.IsExists(chatDb, new NoRightException($"Chat is not group"));
        }

        var alreadyIn = await _chatMemberRepository.GetAll().Select(c => c.User.Id).Where(c => userIds.Contains(c))
            .ToListAsync(cancellationToken);
        var idsToAdd = userIds.Except(alreadyIn);

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat == chatDb)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.AddMembers == true), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
              
        var roleList = new List<Role> { _roleRepository.GetAll().FirstOrDefault(r => r.RoleName == "@everyone" && r.Chat == null)! };
  
        List<ChatMember> chatMembers = new List<ChatMember>();
        foreach (var memberId in idsToAdd)
        {
            var memberDb = await _userRepository.GetByIdAsync(memberId, cancellationToken);
            _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {memberId} not found"));
            chatMembers.Add(new ChatMember
            {
                Chat = chatDb,
                User = memberDb!,
                Role = new List<Role>(roleList)
            });
        }
        await _chatRepository.AddChatMemberAsync(chatMembers, chatDb, cancellationToken);
    }

    public async Task DelMember(int userId, int chatId, int memberId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.DelMembers == true), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));

        var memberDb = await _userRepository.GetByIdAsync(memberId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {memberId} not found"));
        await _chatRepository.DelMemberChatAsync(memberDb!.Id, chatDb!, cancellationToken);

    }
    
    public async Task Delete(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.Id == 2), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        await _chatRepository.DeleteChatAsync(chatDb!, cancellationToken);
    }
    
    public async Task<ChatModel> EditChat(int userId, int chatId, ChatModel chatModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditChat), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<ChatModel, Chat>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(chatModel);
            var roleTargetValue = roleDbProperty.GetValue(chatDb);

            if (roleSourceValue != null && roleSourceValue != "" && !roleSourceValue.Equals(roleTargetValue))
            {
                roleDbProperty.SetValue(chatDb, roleSourceValue);
            }
        }
        
        await _chatRepository.EditChat(chatDb!, cancellationToken);

        return _mapper.Map<ChatModel>(chatDb);
    }
    
    public async Task<List<ChatModel>> FindChatByName(int userId, string chatName, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        chatName = chatName.ToLower();
        var chatList =  await _chatRepository.GetAll()
            .Where(i => i.ChatMembers!.Any(u => u.User.Id == userId) && i.Name.ToLower().StartsWith(chatName))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task<List<ChatModel>> GetAllChats(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        
        var chatList = await _chatRepository.GetAll()
            .Where(chat => chat.ChatMembers!.Any(member => member.User.Id == userId))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task AddRole(int userId, int chatId, RoleModel roleModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var isAlreadyNamed = await _roleRepository.GetAll().Where(r => r.RoleName == roleModel.RoleName).FirstOrDefaultAsync(cancellationToken);
        if (isAlreadyNamed is not null)
        {
            _logger.LogInformation("Role with this name is already created");
            return;
        }
            
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatDb!.Id)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        var role = _mapper.Map<Role>(roleModel);
        role.Chat = chatDb;
        _logger.LogInformation("Role was added in chat");
        await _roleRepository.CreateRole(role, cancellationToken);
    }

    public async Task<RoleModel> GetRoleById(int userId, int chatId, int roleId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatDb!.Id)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        
        var role = await _roleRepository.GetAll()
            .Where(r => r.Id == roleId && r.Chat == chatDb).SingleOrDefaultAsync(cancellationToken);
        _logger.IsExists(role, new RoleNotFoundException($"Role not found"));
        return _mapper.Map<RoleModel>(role);
    }

    public async Task DelRole(int userId, int chatId, int roleId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(role, new RoleNotFoundException($"Role not found"));
        await _roleRepository.DeleteRole(role!, cancellationToken);
    }
    
    
    public async Task<RoleModel> EditRole(int userId, int chatId, int roleId, RoleModel roleModel, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        
        if (chatDb!.Roles!.Contains(roleDb!) == false)
            throw new Exception("This role is not in this chat");
        
        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<RoleModel, Role>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(roleModel);
            var roleTargetValue = roleDbProperty.GetValue(roleDb);

            if (roleSourceValue != null && roleSourceValue != "" && !roleSourceValue.Equals(roleTargetValue))
            {
                roleDbProperty.SetValue(roleDb, roleSourceValue);
            }
        }
        
        var existingIds = new HashSet<int>(roleDb!.ChatMembers.Select(r => r.User.Id));
        var newIds = new HashSet<int>(roleModel.UsersIds);

        var idsToAdd = newIds.Except(existingIds);
        var idsToRemove = existingIds.Except(newIds);

        await SetRole(userId, chatId, roleDb.Id, idsToAdd.ToList(), cancellationToken);
        await UnSetRole(userId, chatId, roleDb.Id, idsToRemove.ToList(), cancellationToken);
        
        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        
        await _roleRepository.EditRole(roleDb!, cancellationToken);
        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        return _mapper.Map<RoleModel>(roleDb);
    }
    
     
    public async Task SetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatDb!.Id)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));
        
        var chatMembersDb = new List<ChatMember>(){};
        foreach (var uId in userIds)
        {
            var chatMemberDb = await _chatMemberRepository.GetAll().Where(m => m.Chat == chatDb && m.User.Id == uId).SingleOrDefaultAsync(cancellationToken);
            _logger.IsExists(chatMemberDb, new UserNotFoundException($"User with this Id {uId} not found"));
            chatMemberDb!.Role.Add(roleDb!);
            chatMembersDb.Add(chatMemberDb!);
        }
        await _chatMemberRepository.SetRole(chatMembersDb!, cancellationToken);
    }
    
    public async Task UnSetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatDb!.Id)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.EditRoles), cancellationToken);
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));
        
        var chatMembers = new List<ChatMember>(){};
        foreach (var uId in userIds)
        {
            var chatMemberDb = await _chatMemberRepository.GetAll().Where(m => m.Chat == chatDb && m.User.Id == uId).SingleOrDefaultAsync(cancellationToken);
            _logger.IsExists(chatMemberDb, new UserNotFoundException($"User with this Id {uId} not found"));
            chatMemberDb!.Role.Remove(roleDb!);
            chatMembers.Add(chatMemberDb!);
        }
        await _chatMemberRepository.SetRole(chatMembers!, cancellationToken);
    }

    public async Task<List<RoleModel>> GetAllChatRoles(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var roles = await _roleRepository.GetAll().Where(r => r.Chat == chatDb).ToListAsync(cancellationToken);
        return _mapper.Map<List<RoleModel>>(roles);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, int roleId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.IsExists(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Role.Any(r => r == roleDb) && c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.IsExists(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }
}