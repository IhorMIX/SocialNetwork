namespace SocialNetwork.BL.Models;

public class ReadMessageModel
{
    public DateTime ReadAt { get; set; }
    
    public int ChatMemberId { get; set; }

    public int MessageId { get; set; }
    
    public MessageModel Message { get; set; }

    public ChatMemberModel ChatMember { get; set; }
}