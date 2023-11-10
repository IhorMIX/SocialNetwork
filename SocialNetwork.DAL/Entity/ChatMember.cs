namespace SocialNetwork.DAL.Entity;

public class ChatMember : BaseEntity
{
    public Chat Chat { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Role> Role { get; set; } = null!;
    
    public ICollection<Message> MessagesSent { get; set; } = null!;
    
    public ICollection<Reaction> Reactions { get; set; } = null!;

}