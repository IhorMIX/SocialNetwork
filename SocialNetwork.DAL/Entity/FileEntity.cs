namespace SocialNetwork.DAL.Entity;

public class FileEntity : BaseEntity
{
    public string FilePath { get; set; }
    public int MessageId { get; set; }
    public Message Message { get; set; }
}