namespace SocialNetwork.BL.Models;

public class MessageModel : BaseModel
{
    public string Text { get; set; }
    public string Files { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    
    public int AuthorId { get; set; }
    public int ChatId { get; set; }
    public int? ToReplyMessageId { get; set; }
    
    public ChatMemberModel? Author { get; set; }
    public ChatModel Chat { get; set; }
    public MessageModel? ToReplyMessage { get; set; }
    
    public ICollection<ReactionModel>? Reactions { get; set; }
}