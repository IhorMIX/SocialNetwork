using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class BannedUserViewModel
{
    public int id { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public ProfileBannedUserModel Profile { get; set; }
}