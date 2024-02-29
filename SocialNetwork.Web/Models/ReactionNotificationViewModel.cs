namespace SocialNetwork.Web.Models;

public class ReactionNotificationViewModel : ChatNotificationViewModel
{
    public int ReactionId { get; set; }
    public ReactionViewModel Reaction { get; set; } = null!;
}