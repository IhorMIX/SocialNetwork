namespace SocialNetwork.BLL.Models;

public class UserPostModel : BasePostModel
{
    public int UserId { get; set; }
    public UserModel User { get; set; } = null!;
}