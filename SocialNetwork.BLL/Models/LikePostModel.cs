namespace SocialNetwork.BLL.Models;

public class LikePostModel : BaseModel
{
    public int PostId { get; set; }
    
    public BasePostModel Post { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public UserModel User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}