namespace SocialNetwork.DAL.Entity;

public class BasePostEntity : BaseEntity
{
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<FileInPost> Files { get; set; } = null!;
    public IEnumerable<LikePost> Likes { get; set; } = null!;
    public IEnumerable<CommentPost> Comments { get; set; } = null!;
    
}