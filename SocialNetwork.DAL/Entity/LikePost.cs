namespace SocialNetwork.DAL.Entity;

public class LikePost : BaseEntity
{
    public int PostId { get; set; }
    
    public BasePostEntity Post { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}