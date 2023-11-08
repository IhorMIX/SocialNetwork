namespace SocialNetwork.BLL.Models;

public class ChatModel : BaseModel
{
    public string Name { get; set; }
    
    public string Logo { get; set; }
    
    public bool isGroup { get; set; }

    public ICollection<ChatMemberModel>? ChatMembers { get; set; }
    public ICollection<RoleModel>? Roles { get; set; }
    
}