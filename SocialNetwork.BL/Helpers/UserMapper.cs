using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using LoginType = SocialNetwork.BL.Models.Enums.LoginType;
using OnlineStatus = SocialNetwork.BL.Models.Enums.OnlineStatus;
using Sex = SocialNetwork.BL.Models.Enums.Sex;

namespace SocialNetwork.BL.Helpers;

//TODO: Igor, you need to delete this class after implementing automapper
public static class UserMapper
{
    public static UserModel? ConvertUserToBlModel(User? user)
    {
        if (user is null)
        {
            return null;
        }

        return new UserModel()
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            IsEnabled = user.IsEnabled,
            OnlineStatus = (OnlineStatus)user.OnlineStatus,

            Profile = new ProfileModel()
            {
                Id = user.Profile.Id,
                Birthday = user.Profile.Birthday,
                Description = user.Profile.Description,
                Email = user.Profile.Email,
                Name = user.Profile.Name,
                Sex = (Sex)user.Profile.Sex,
                Surname = user.Profile.Surname,
                AvatarImage = user.Profile.AvatarImage
            }
        };
    }
    
    public static User ConvertUserToDalModel(UserModel user)
    {
        if (user is null)
        {
            return new User();
        }

        return new User()
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            IsEnabled = user.IsEnabled,
            OnlineStatus = (DAL.Entity.Enums.OnlineStatus)user.OnlineStatus,

            Profile = new Profile()
            {
                Id = user.Profile.Id,
                Birthday = user.Profile.Birthday,
                Description = user.Profile.Description,
                Email = user.Profile.Email,
                Name = user.Profile.Name,
                Sex = (DAL.Entity.Enums.Sex)user.Profile.Sex,
                Surname = user.Profile.Surname,
                AvatarImage = user.Profile.AvatarImage
            }
        };
    }
}