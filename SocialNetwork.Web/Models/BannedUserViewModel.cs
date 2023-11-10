using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class BannedUserViewModel
{
    public int Id { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public ProfileBannedUserModel Profile { get; set; } = null!;
}