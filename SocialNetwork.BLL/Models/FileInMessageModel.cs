namespace SocialNetwork.BLL.Models;

public class FileInMessageModel : BaseFileModel
{
    public int MessageId { get; set; }
    public MessageModel Message { get; set; } = null!;
}