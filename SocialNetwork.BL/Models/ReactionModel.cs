namespace SocialNetwork.BL.Models;

public class ReactionModel : BaseModel
{
    public string Type { get; set; }
    
    public int MessageId { get; set; }
    
    public int AuthorId { get; set; }
    
    public ChatMemberModel Author { get; set; }
    
    public MessageModel Message { get; set; }
}