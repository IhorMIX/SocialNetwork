namespace SocialNetwork.DAL.Entity;

public class ChatNotification : BaseNotificationEntity
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
}