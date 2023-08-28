namespace SocialNetwork.Web.Models;

public class ChatEditModel
{
    public string Name { get; set; }
    
    public string Logo { get; set; }
    
    public ICollection<int> ChatMembersToRemove { get; set; } 
    public ICollection<int> RolesToRemove { get; set; } 
}