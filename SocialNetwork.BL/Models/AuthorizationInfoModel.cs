using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.BL.Models;

public class AuthorizationInfoModel : BaseModel
{
    public string RefreshToken { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public LoginType LoginType { get; set; }
}