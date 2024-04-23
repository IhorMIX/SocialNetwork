namespace SocialNetwork.DAL.Entity;

public class UserPost : BasePostEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}