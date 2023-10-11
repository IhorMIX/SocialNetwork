using System.Text;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.Test.Helpers;

public static class UserModelHelper
{
    
    private static readonly Random _random = new();

    public static async Task<UserModel> CreateUserDateAsync()
    {
        Random random = new Random();
        return new UserModel()
        {

            Login = GenerateRandomLogin(8, 60),
            Password = "Password",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = GenerateRandomLogin(8, 60) + "@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
    }
    
   

    public static string GenerateRandomLogin(int minLength, int maxLength)
    {
        int loginLength = _random.Next(minLength, maxLength + 1);
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder loginBuilder = new StringBuilder();

        for (int i = 0; i < loginLength; i++)
        {
            int randomIndex = _random.Next(chars.Length);
            loginBuilder.Append(chars[randomIndex]);
        }

        return loginBuilder.ToString();
    }

    public static async Task<UserModel> CreateTestDataAsync(IUserService userService)
    {
        var user = await CreateUserDateAsync();
        await userService.CreateUserAsync(user);
        await userService.ActivateUser(user.Id);
        return user;
    }

}