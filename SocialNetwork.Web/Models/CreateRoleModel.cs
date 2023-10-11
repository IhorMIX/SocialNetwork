using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.Web.Models;

public class CreateRoleModel
{
    public int ChatId { get; set; }
    public string RoleName { get; set; }
    
    public string RoleColor { get; set; }
    
    public List<ChatAccess> RoleAccesses { get; set; } = new ();
    public int Rank { get; set; }
}