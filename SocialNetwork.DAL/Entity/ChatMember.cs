namespace SocialNetwork.DAL.Entity;

public class ChatMember : BaseEntity
{
    public Chat Chat { get; set; }
    public User User { get; set; }
    public ICollection<Role> Role { get; set; }
}