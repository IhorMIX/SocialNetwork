namespace SocialNetwork.Web.Models;

public class MessageReadStatusViewModel
{
    public int ChatMemberId { get; set; }
    public int MessageId { get; set; }

    public bool IsRead { get; set; }

    public DateTime ReadAt { get; set; }

    public ChatMemberViewModel ChatMember { get; set; }
}