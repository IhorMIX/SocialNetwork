namespace SocialNetwork.BLL.Models;

public class FileInPostModel : BaseFileModel
{
    public int PostId { get; set; }
    public BaseFileModel Post { get; set; }
}