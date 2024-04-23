namespace SocialNetwork.DAL.Entity;

public class BasePostEntity : BaseEntity
{
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<FileInPost> Files { get; set; } = null!;
    
}