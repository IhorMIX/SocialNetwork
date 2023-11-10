using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;
    public string RoleColor { get; set; } = null!;
    
    public ICollection<RoleChatAccess> RoleAccesses { get; set; } = null!;
    
    public ICollection<ChatMember> ChatMembers { get; set; } = null!;
    public Chat? Chat { get; set; }
    public int Rank { get; set; }
}
    