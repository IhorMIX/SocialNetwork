namespace SocialNetwork.BLL.Models;

public abstract class NotificationModel : BaseModel
{
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}