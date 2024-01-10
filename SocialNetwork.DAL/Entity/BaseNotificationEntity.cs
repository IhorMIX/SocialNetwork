namespace SocialNetwork.DAL.Entity;

public abstract class BaseNotificationEntity : BaseEntity
{
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}