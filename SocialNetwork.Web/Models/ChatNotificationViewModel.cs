namespace SocialNetwork.Web.Models;

public class ChatNotificationViewModel : BaseNotificationViewModel
{
    //public int ChatId { get; set; }
    public ChatViewModel Chat { get; set; } = null!;
}