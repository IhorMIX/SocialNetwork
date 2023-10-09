using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Role : BaseEntity
{
    public string RoleName { get; set; }
    public string RoleColor { get; set; }
    
    public ICollection<ChatAccess> RoleAccesses { get; set; } = new List<ChatAccess>();
    
    public ICollection<ChatMember> ChatMembers { get; set; }
    public Chat? Chat { get; set; }
    public int Rank { get; set; }
}
    