namespace SocialNetwork.BLL.Models;

public class CommentPostModel : BaseModel
{
    public int PostId { get; set; }
    
    public BasePostModel Post { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public UserModel User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public string Text { get; set; } = null!;
    
    public int ToReplyCommentId { get; set; }
    public CommentPostModel? ToReplyComment { get; set; }
}