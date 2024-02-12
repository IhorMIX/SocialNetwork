namespace SocialNetwork.DAL.Entity;

public class ReactionNotification : ChatNotification
{
    public int ReactionId { get; set; }
    public Reaction Reaction { get; set; } = null!;
}