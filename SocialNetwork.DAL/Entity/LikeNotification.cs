namespace SocialNetwork.DAL.Entity;

public class LikeNotification : NotificationEntity
{
    public int LikePostId { get; set; }

    public LikePost LikePost { get; set; } = null!;
}