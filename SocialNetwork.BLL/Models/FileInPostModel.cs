namespace SocialNetwork.BLL.Models;

public class FileInPostModel : BaseFileModel
{
    public int PostId { get; set; }
    public BasePostModel Post { get; set; } = null!;
}