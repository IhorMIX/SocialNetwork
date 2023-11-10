using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.Web.Models;

public class RoleEditModel
{
    public string RoleName { get; set; } = null!;
    public string RoleColor { get; set; } = null!;

    public List<ChatAccess> RoleAccesses { get; set; } = new ();
    
    public ICollection<int> UsersIds { get; set; } = null!;
    
    public int Rank { get; set; }
}