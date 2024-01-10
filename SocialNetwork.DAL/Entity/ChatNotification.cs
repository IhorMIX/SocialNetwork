namespace SocialNetwork.DAL.Entity;

public class ChatNotification : BaseNotificationEntity
{
    public int ChatId { get; set; }
    public string ChatName { get; set; } = null!;
    public string Logo { get; set; } = null!;
    public int UserInitiatorId { get; set; }
}