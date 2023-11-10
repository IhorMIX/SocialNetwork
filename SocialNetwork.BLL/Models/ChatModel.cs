namespace SocialNetwork.BLL.Models;

public class ChatModel : BaseModel
{
    public string Name { get; set; } = null!;
    
    public string Logo { get; set; } = null!;
    
    public bool IsGroup { get; set; }

    public ICollection<ChatMemberModel>? ChatMembers { get; set; }
    public ICollection<RoleModel>? Roles { get; set; }
    
}