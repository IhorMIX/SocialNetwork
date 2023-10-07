namespace SocialNetwork.Web.Models;

public class IdsForRoleModel
{
    public int ChatId { get; set; }
    public int RoleId { get; set; }
    public List<int>? MemberIds { get; set; }
}