namespace SocialNetwork.BLL.Models;

public class ChatNotificationModel : BaseNotificationModel
{
    public int ChatId { get; set; }
    public ChatModel Chat { get; set; } = null!;
}