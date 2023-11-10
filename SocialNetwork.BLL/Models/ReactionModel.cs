namespace SocialNetwork.BLL.Models;

public class ReactionModel : BaseModel
{
    public string Type { get; set; } = null!;
    
    public int MessageId { get; set; }
    
    public int AuthorId { get; set; }
    
    public ChatMemberModel Author { get; set; } = null!;
    
    public MessageModel Message { get; set; } = null!;
}