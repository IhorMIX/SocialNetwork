using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Role : BaseEntity
{
    public string RoleName { get; set; }
    public string RoleColor { get; set; }
    
    public ICollection<RoleChatAccess> RoleAccesses { get; set; }
    
    public ICollection<ChatMember> ChatMembers { get; set; }
    public Chat? Chat { get; set; }
    public int Rank { get; set; }
}
    