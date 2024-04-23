namespace SocialNetwork.BLL.Models;

public class FriendRequestModel : BaseRequestModel
{
        
        public int ToUserId { get; set; }
        public UserModel ToUser { get; set; } = null!;
}