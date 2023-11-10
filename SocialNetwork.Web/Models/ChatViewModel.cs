namespace SocialNetwork.Web.Models;

public class ChatViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Logo { get; set; } = null!;
    
    public bool IsGroup { get; set; }
}