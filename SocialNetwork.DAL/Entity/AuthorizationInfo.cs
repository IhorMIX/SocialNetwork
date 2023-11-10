using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class AuthorizationInfo : BaseEntity
{
    public string RefreshToken { get; set; }  = null!;

    public DateTime? ExpiredDate { get; set; }

    public LoginType LoginType { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}