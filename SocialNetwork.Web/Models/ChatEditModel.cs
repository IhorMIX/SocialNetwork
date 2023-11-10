namespace SocialNetwork.Web.Models;

public class ChatEditModel
{
    public int ChatId { get; set; }
    public string Name { get; set; } = null!;
    
    public string Logo { get; set; } = null!;
}