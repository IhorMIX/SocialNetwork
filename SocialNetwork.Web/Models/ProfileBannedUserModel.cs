using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class ProfileBannedUserModel
{
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public string AvatarImage { get; set; }

    public Sex Sex { get; set; }
}