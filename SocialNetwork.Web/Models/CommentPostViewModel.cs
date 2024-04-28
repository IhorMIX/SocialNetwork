namespace SocialNetwork.Web.Models;

public class CommentPostViewModel
{
    public int Id { get; set; }
    public int PostId { get; set; }
    
    public int UserId { get; set; }
    
    public UserViewModel User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public string Text { get; set; } = null!;
    
    public int ToReplyCommentId { get; set; }
}