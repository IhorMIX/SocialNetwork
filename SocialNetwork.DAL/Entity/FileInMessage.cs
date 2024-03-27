namespace SocialNetwork.DAL.Entity;

public class FileInMessage : FileEntity
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
}