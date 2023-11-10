using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class FriendViewModel
{
    public int Id { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public ProfileFriendViewModel Profile { get; set; } = null!;
}