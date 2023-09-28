namespace SocialNetwork.DAL.Entity;

public class ReadMessage : BaseEntity
{
    public DateTime ReadAt { get; set; }
    
    public int ChatMemberId { get; set; }

    public int MessageId { get; set; }
    
    public Message Message { get; set; }

    public ChatMember ChatMember { get; set; }
}