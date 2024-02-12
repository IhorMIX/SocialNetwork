namespace SocialNetwork.BLL.Models;

public class MessageNotificationModel : ChatNotificationModel
{
    public int MessageId { get; set; }
    public MessageModel Message { get; set; } = null!;
}