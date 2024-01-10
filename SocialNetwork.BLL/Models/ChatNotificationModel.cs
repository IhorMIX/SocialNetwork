namespace SocialNetwork.BLL.Models;

public class ChatNotificationModel : NotificationModel
{
    public int ChatId { get; set; }
    public string ChatName { get; set; } = null!;
    public string Logo { get; set; } = null!;
    public int UserInitiatorId { get; set; }
}