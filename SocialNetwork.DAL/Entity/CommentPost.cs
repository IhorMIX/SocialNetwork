namespace SocialNetwork.DAL.Entity;

public class CommentPost : BaseEntity
{ 
    public int PostId { get; set; }
    
    public BasePostEntity Post { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public string Text { get; set; } = null!;
    
    public int? ToReplyCommentId { get; set; }
    public CommentPost? ToReplyComment { get; set; }
    
}