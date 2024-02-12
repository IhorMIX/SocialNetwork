namespace SocialNetwork.BLL.Models;

public abstract class BaseNotificationModel : BaseModel
{
    public string NotificationMessage { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int ToUserId { get; set; }
    
    public int InitiatorId { get; set; }
    public UserModel Initiator { get; set; } = null!;
}