namespace SocialNetwork.DAL.Entity;

public class FriendRequest : BaseRequestEntity
{
    public int ToUserId { get; set; }
    public User ToUser { get; set; } = null!;
    
}