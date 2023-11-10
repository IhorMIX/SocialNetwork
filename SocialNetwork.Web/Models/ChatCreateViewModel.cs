namespace SocialNetwork.Web.Models;

public class ChatCreateViewModel
{
    public string Name { get; set; } = null!;
    
    public string Logo { get; set; } = null!;
    
    public bool IsGroup { get; set; }
}