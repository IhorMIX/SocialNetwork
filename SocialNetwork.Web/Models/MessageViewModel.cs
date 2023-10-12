using SocialNetwork.BL.Models;

namespace SocialNetwork.Web.Models;

public class MessageViewModel
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Files { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    
    public int AuthorId { get; set; }
    public int ChatId { get; set; }
    public int ToReplyMessageId { get; set; }
    
    public ICollection<ReactionModel>? Reactions { get; set; }
}