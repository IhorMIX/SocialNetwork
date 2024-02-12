namespace SocialNetwork.DAL.Entity;

public class MessageReadStatus : BaseEntity
{
    public int ChatMemberId { get; set; }
    public int MessageId { get; set; }

    public bool IsRead { get; set; }

    public DateTime ReadAt { get; set; }

    public ChatMember ChatMember { get; set; } = null!;
    public Message Message { get; set; } = null!;
}