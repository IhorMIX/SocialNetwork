namespace SocialNetwork.Web.Models;

public class UserCreateViewModel
{
    public string Login { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public ProfileCreateViewModel Profile { get; set; } = null!;
}