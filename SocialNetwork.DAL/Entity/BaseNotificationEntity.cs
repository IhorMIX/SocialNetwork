namespace SocialNetwork.DAL.Entity;

public class BaseNotificationEntity : BaseEntity
{
    public string NotificationMessage { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int ToUserId { get; set; }
    
    public int InitiatorId { get; set; }
    public User Initiator { get; set; } = null!;
    
}