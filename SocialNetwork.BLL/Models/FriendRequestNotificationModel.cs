namespace SocialNetwork.BLL.Models;

public class FriendRequestNotificationModel : NotificationModel
{ 
    public int FromUserModelId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string AvatarImage { get; set; } = null!;
}