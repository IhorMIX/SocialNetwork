using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class FriendViewModel
{
    public int id { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public ProfileFriendViewModel Profile { get; set; }
}