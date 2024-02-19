namespace SocialNetwork.Web.Models;

public class ChatNotificationViewModel : BaseNotificationViewModel
{
    public ChatViewModel Chat { get; set; } = null!;
}