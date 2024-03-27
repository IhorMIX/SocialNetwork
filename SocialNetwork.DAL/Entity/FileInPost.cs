namespace SocialNetwork.DAL.Entity;

public class FileInPost : FileEntity
{
    public int PostId { get; set; }
    public BasePostEntity Post { get; set; } = null!;
}