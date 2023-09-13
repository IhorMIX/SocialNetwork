using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IChatRepository : IBasicRepository<Chat>
{
    Task<int> CreateChat(Chat chat, CancellationToken cancellationToken = default);
    
    Task DeleteChatAsync(Chat chat, CancellationToken cancellationToken = default);
    Task AddChatMemberAsync(ChatMember сhatMember, Chat chat, CancellationToken cancellationToken = default);
    Task AddChatMemberAsync(List<ChatMember> сhatMembers, Chat chat, CancellationToken cancellationToken = default);
    Task DelChatMemberAsync(int userId, Chat chat, CancellationToken cancellationToken = default);
    Task DelChatMemberAsync(List<ChatMember> chatMembers, Chat chat, CancellationToken cancellationToken = default);
    public Task EditChat(Chat role, CancellationToken cancellationToken = default);
}