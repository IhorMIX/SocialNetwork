using System.Text.Json.Serialization;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.BLL.Models;

public class RoleModel : BaseModel
{
    public string RoleName { get; set; } = null!;
    public string RoleColor { get; set; } = null!;

    public List<ChatAccess> RoleAccesses { get; set; } = new();

    public ICollection<int> UsersIds { get; set; } = null!;
    public ChatModel? Chat { get; set; }
    public int Rank { get; set; }
}