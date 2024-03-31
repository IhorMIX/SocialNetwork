namespace SocialNetwork.DAL.Entity;

public class ChatNotification : NotificationEntity
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
}