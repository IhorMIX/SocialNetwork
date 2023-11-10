namespace SocialNetwork.Web.Models;

public class DelChatMembersModel
{
    public int ChatId { get; set; }
    public List<int> MemberIds { get; set; } = null!;
}