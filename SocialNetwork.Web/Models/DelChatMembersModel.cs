namespace SocialNetwork.Web.Models;

public class DelChatMembersModel
{
    public int ChatId { get; set; }
    public List<int> MeberIds { get; set; }
}