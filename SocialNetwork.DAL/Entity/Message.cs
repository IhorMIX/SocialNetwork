using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.DAL.Entity;

public class Message : BaseEntity
{
    public string Text { get; set; }
    public ICollection<FileEntity>? Files { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    
    public int AuthorId { get; set; }
    public int ChatId { get; set; }
    public int? ToReplyMessageId { get; set; }
    
    public ChatMember? Author { get; set; }
    public Chat Chat { get; set; }
    public Message? ToReplyMessage { get; set; }
    
    public ICollection<Reaction>? Reactions { get; set; }
}