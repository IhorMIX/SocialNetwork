using System.Text.Json.Serialization;

namespace SocialNetwork.BLL.Models;

public class ChatMemberModel : BaseModel
{
    public ChatModel Chat { get; set; }
    public UserModel User { get; set; }
    public ICollection<RoleModel> Role { get; set; }
}