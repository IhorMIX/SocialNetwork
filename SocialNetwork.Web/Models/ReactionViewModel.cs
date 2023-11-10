namespace SocialNetwork.Web.Models;

public class ReactionViewModel
{
    private int Id { get; set; }
    public string Type { get; set; } = null!;
    public UserViewModel Author { get; set; } = null!;
}