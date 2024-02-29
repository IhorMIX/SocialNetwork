namespace SocialNetwork.Web.Models;

public class ReactionViewModel
{
    private int Id { get; set; }
    public string Type { get; set; } = null!;
    public ChatMemberViewModel Author { get; set; } = null!;
}