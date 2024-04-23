namespace SocialNetwork.DAL.Entity;

public class FileInPost : BaseFileEntity
{
    public int PostId { get; set; }
    public BasePostEntity Post { get; set; } = null!;
}