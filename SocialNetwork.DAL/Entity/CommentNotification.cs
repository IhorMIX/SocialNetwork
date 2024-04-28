namespace SocialNetwork.DAL.Entity;

public class CommentNotification : NotificationEntity
{
    public int CommentPostId { get; set; }
    
    public CommentPost CommentPost { get; set; } = null!;
}