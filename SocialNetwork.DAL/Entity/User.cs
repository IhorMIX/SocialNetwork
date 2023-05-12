using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class User : BaseEntity
{
    public string Login { get; set; }

    public string Password { get; set; }

    public OnlineStatus OnlineStatus { get; set; }

    public bool IsEnabled { get; set; }

    public int ProfileId { get; set; }

    public int AuthorizationInfoId { get; set; }

    public Profile Profile { get; set; }

    public AuthorizationInfo AuthorizationInfo { get; set; }
}