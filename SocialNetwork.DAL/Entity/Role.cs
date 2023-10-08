using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Role : BaseEntity
{
    public string RoleName { get; set; }
    public string RoleColor { get; set; }
    
    public bool SendMessages { get; set; }
    public bool SendAudioMess { get; set; }
    public bool SendFiles { get; set; }
    
    public bool EditRoles { get; set; }
    public bool AddMembers { get; set; }
    public bool DelMembers { get; set; }
    public bool MuteMembers { get; set; }
    public bool DelMessages { get; set; }
    public bool EditNicknames { get; set; }
    
    public bool EditChat { get; set; }

    public List<ChatAccess> RoleAccesses { get; set; } = new ();
    
    public ICollection<ChatMember> ChatMembers { get; set; }
    public Chat? Chat { get; set; }
    public int Rank { get; set; }
}
    