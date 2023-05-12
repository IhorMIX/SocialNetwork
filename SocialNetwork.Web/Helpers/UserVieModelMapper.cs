using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Helpers;

public static class UserVieModelMapper
{
    //TODO: Igor, you need to delete this class after implementing automapper
    public static UserModel ConvertToBlModel(UserCreateViewModel user)
    {
        Enum.TryParse<Sex>(user.Profile.Sex, out var sex);
        return new UserModel()
        {
            Login = user.Login,
            Password = user.Password,
            Profile = new ProfileModel()
            {
                Birthday = user.Profile.Birthday,
                Description = user.Profile.Description,
                Email = user.Profile.Email,
                Name = user.Profile.Name,
                Sex = sex,
                Surname = user.Profile.Surname,
                AvatarImage = user.Profile.AvatarImage
            }
        };
    }
}