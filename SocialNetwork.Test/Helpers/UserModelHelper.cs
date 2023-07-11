using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.Test.Helpers;

public static class UserModelHelper
{
    public static UserModel CreateTestData()
    {
        Random random = new Random();
        var num = random.Next(100, 1000);
        return new UserModel()
        {
            Login = "User" + num.ToString(),
            Password = "Password",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User" + num.ToString() + "@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
    }
    
}