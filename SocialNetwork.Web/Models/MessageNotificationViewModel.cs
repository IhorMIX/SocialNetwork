namespace SocialNetwork.Web.Models;

public class MessageNotificationViewModel : ChatNotificationViewModel
{
    public MessageViewModel Message { get; set; } = null!;
}