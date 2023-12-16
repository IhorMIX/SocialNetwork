namespace SocialNetwork.DAL.Entity;

public class FriendRequestNotification : BaseNotificationEntity
{
    public int FriendRequestId { get; set; }
    public int FromUserId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string AvatarImage { get; set; } = null!;
}