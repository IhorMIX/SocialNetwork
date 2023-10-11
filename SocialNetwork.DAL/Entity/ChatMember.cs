namespace SocialNetwork.DAL.Entity;

public class ChatMember : BaseEntity
{
    public Chat Chat { get; set; }
    public User User { get; set; }
    public ICollection<Role> Role { get; set; }
    
    public ICollection<Message>? MessagesSent { get; set; }
    
    public ICollection<Reaction>? Reactions { get; set; }

}