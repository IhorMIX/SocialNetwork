namespace SocialNetwork.Web.Models;

public class LikePostViewModel
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public UserViewModel User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}