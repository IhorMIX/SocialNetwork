namespace SocialNetwork.DAL.Entity;

public class FriendRequest : BaseEntity
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
    
}