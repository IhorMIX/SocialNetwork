namespace SocialNetwork.DAL.Entity;

public class Reaction : BaseEntity
{
    public string Type { get; set; }
    
    public int MessageId { get; set; }
    
    public int AuthorId { get; set; }
    
    public ChatMember Author { get; set; }
    
    public Message Message { get; set; }
}