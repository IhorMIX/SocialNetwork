using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.DAL.Entity;

public class Message : BaseEntity
{
    public string Text { get; set; } = null!;
    public ICollection<FileEntity>? Files { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }

    public int CreatorId { get; set; }
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public int? ToReplyMessageId { get; set; }

    public User Creator { get; set; } = null!;
    public ChatMember Sender { get; set; } = null!;
    public Chat Chat { get; set; } = null!;
    public Message? ToReplyMessage { get; set; }
    public ICollection<Reaction> Reactions { get; set; } = null!;
    public ICollection<MessageReadStatus> MessageReadStatuses { get; set; } = null!;
}