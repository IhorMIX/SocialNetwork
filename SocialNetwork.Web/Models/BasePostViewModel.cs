namespace SocialNetwork.Web.Models;

public class BasePostViewModel
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}