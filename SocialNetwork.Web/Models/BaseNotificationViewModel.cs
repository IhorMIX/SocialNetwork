namespace SocialNetwork.Web.Models;

public class BaseNotificationViewModel
{
    public int Id { get; set; }
    public string NotificationMessage { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int ToUserId { get; set; }
    
    public int InitiatorId { get; set; }
    public UserViewModel Initiator { get; set; } = null!;
}