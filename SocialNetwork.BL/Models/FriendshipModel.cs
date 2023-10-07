namespace SocialNetwork.BL.Models;

public class FriendshipModel
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public UserModel? UserModel { get; set; }
    public UserModel? FriendUserModel { get; set; }
}