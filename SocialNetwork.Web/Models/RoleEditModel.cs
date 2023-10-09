using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.Web.Models;

public class RoleEditModel
{
    public string RoleName { get; set; }
    public string RoleColor { get; set; }

    public List<ChatAccess> RoleAccesses { get; set; } = new ();
    
    public ICollection<int> UsersIds { get; set; }
    
    public int Rank { get; set; }
}