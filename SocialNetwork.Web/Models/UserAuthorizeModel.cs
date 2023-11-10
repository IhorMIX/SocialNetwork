namespace SocialNetwork.Web.Models;

public class UserAuthorizeModel
{
    public string Login { get; set; } = null!;
    
    public string Password { get; set; } = null!;

    public bool IsNeedToRemember { get; set; }
}