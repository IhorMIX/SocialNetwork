namespace SocialNetwork.DAL.Entity;

public class MessageNotification : ChatNotification
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
}