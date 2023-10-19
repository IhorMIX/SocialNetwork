namespace SocialNetwork.Web.Models;

public class ReactionViewModel
{
    private int Id { get; set; }
    public string Type { get; set; }
    public UserViewModel Author { get; set; }
}