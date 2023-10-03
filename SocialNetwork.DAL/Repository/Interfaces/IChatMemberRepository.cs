using System.Security.Cryptography.X509Certificates;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IChatMemberRepository : IBasicRepository<ChatMember>
{
    public Task SetRole(List<ChatMember> chatMembers, CancellationToken cancellationToken = default);
    public Task<ChatMember?> GetByUserIdAndChatId(int userId, int chatId, CancellationToken cancellationToken = default);
}