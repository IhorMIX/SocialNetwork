namespace SocialNetwork.Web.Models;

public class RoleUpdateModel
{
    public int ChatId { get; set; }
    public int RoleId { get; set; }
    public RoleEditModel RoleModel { get; set; } = null!;
}