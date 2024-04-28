namespace SocialNetwork.BLL.Models;

public class BasePostModel : BaseModel
{
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<FileInPostModel> Files { get; set; } = null!;
    
    public ICollection<LikePostModel> Likes { get; set; } = null!;
    public ICollection<CommentPostModel> Comments { get; set; } = null!;
}