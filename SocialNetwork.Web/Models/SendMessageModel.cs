namespace SocialNetwork.Web.Models;

public class SendMessageModel
{
    public int ChatId { get; set; }
    public string Text { get; set; }
    public string Files { get; set; }
}