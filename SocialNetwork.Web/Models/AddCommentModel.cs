namespace SocialNetwork.Web.Models;

public class AddCommentModel
{
    public string Text { get; set; } = null!;
    public int PostId { get; set; }
}