namespace SocialNetwork.Web.Models;

public class ReplyOnCommentModel
{
    public string Text { get; set; } = null!;
    public int CommentId { get; set; }
}