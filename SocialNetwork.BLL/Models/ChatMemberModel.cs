using System.Text.Json.Serialization;

namespace SocialNetwork.BLL.Models;

public class ChatMemberModel : BaseModel
{
    public ChatModel Chat { get; set; } = null!;
    public UserModel User { get; set; } = null!;
    public ICollection<RoleModel> Role { get; set; } = null!;
}