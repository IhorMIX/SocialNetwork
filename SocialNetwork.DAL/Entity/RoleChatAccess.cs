using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class RoleChatAccess : BaseEntity
{
    public ChatAccess ChatAccess { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}