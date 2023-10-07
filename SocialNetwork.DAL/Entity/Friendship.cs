namespace SocialNetwork.DAL.Entity;

public class Friendship : BaseEntity
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public User User { get; set; }
    public User FriendUser { get; set; }
}