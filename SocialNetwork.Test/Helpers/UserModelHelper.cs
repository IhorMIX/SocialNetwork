using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.Test.Helpers;

public static class UserModelHelper
{
    public static async Task<UserModel> CreateTestData()
    {
        Random random = new Random();
        return new UserModel()
        {
            Login = "User" + random.Next(100, 10000),
            Password = "Password",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User" + random.Next(100, 10000) + "@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
    }
    
}