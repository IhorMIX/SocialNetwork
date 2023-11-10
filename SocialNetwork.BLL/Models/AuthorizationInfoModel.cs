using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.BLL.Models;

public class AuthorizationInfoModel : BaseModel
{
    public string RefreshToken { get; set; } = null!;

    public DateTime? ExpiredDate { get; set; }

    public LoginType LoginType { get; set; }
}