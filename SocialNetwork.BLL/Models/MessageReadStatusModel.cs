using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BLL.Models;

public class MessageReadStatusModel : BaseModel
{
    public int ChatMemberId { get; set; }
    public int MessageId { get; set; }

    public bool IsRead { get; set; }

    public DateTime ReadAt { get; set; }

    public ChatMemberModel ChatMember { get; set; } = null!;
}