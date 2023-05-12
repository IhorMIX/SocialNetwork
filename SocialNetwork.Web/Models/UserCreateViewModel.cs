namespace SocialNetwork.Web.Models;

public class UserCreateViewModel
{
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public ProfileCreateViewModel Profile { get; set; }
}