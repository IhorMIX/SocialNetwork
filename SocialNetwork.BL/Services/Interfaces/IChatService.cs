using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IChatService : IBaseService<ChatModel>
{
    Task CreateP2PChat(int userId, int user2Id, ChatModel chatModel, CancellationToken cancellationToken = default);
    Task<ChatModel> CreateGroupChat(int userId, ChatModel chatModel, CancellationToken cancellationToken = default);
    Task AddUsers(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default);
    Task DelMember(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default);
    
    Task<ChatModel> EditChat(int userId, int chatId, ChatModel chatModel, CancellationToken cancellationToken = default);
    Task DeleteChat(int userId, int chatId, CancellationToken cancellationToken = default);
    Task<List<ChatModel>> FindChatByName(int userId, string chatName, CancellationToken cancellationToken = default);
    Task<List<ChatModel>> GetAllChats(int userId, CancellationToken cancellationToken = default);

    Task AddRole(int userId, int chatId, RoleModel roleModel, CancellationToken cancellationToken = default);
    Task<RoleModel> GetRoleById(int userId, int chatId, int roleId, CancellationToken cancellationToken = default);
    Task DelRole(int userId, int chatId, int roleId, CancellationToken cancellationToken = default);
    Task<RoleModel> EditRole(int userId, int chatId, int roleId, RoleModel roleModel, CancellationToken cancellationToken = default);

    Task SetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default);
    Task UnSetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default);

    Task<List<RoleModel>> GetAllChatRoles(int userId, int chatId, CancellationToken cancellationToken = default);

    Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, int roleId, CancellationToken cancellationToken = default);
    
    Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, CancellationToken cancellationToken = default);
    Task<List<RoleModel>> EditRolesRank(int userId, int chatId, List<RoleModel> roleModels, CancellationToken cancellationToken = default);

    Task LeaveChat(int userId, int chatId, CancellationToken cancellationToken = default);
    Task MakeHost(int userId, int chatId, int chatMemberId, CancellationToken cancellationToken = default);
}