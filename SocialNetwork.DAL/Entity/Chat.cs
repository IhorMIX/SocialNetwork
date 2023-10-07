namespace SocialNetwork.DAL.Entity;

public class Chat : BaseEntity
{

    public string Name { get; set; }
    
    public string Logo { get; set; }
    
    public bool IsGroup { get; set; }
    
    public ICollection<ChatMember>? ChatMembers { get; set; }
    public ICollection<Role>? Roles { get; set; }
}