using SocialNetwork.BL.Models.Enums;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.Models;

public class UserModel : BaseModel
{
    public string Login { get; set; }

    public string Password { get; set; }

    public OnlineStatus OnlineStatus { get; set; }

    public bool IsEnabled { get; set; }
    

    public ProfileModel Profile { get; set; }

    public AuthorizationInfoModel AuthorizationInfo { get; set; }
    
    public IEnumerable<Friendship>? Friends { get; set; }
}