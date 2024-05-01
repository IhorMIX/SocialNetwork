using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.Web.Models;

public class RoleGroupViewModel
{
    public int Id { get; set; }
    public string RoleName { get; set; } = null!;
    public string RoleColor { get; set; } = null!;

    public List<GroupAccess> RoleAccesses { get; set; } = new();
    public ICollection<int> UsersIds { get; set; } = null!;

    public int Rank { get; set; }
}