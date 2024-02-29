namespace SocialNetwork.BLL.Models;

public class ReactionNotificationModel : ChatNotificationModel
{
    public int ReactionId { get; set; }
    public ReactionModel Reaction { get; set; } = null!;
}