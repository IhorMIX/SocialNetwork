namespace SocialNetwork.DAL.Entity;

public class Reaction : BaseEntity
{
    public string Type { get; set; } = null!;
    
    public int MessageId { get; set; }
    
    public int AuthorId { get; set; }
    
    public ChatMember Author { get; set; } = null!;
    
    public Message Message { get; set; } = null!;
}