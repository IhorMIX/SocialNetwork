namespace SocialNetwork.DAL.Entity;

public class FileInMessage : BaseFileEntity
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
}