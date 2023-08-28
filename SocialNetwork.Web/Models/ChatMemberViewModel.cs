namespace SocialNetwork.Web.Models;

public class ChatMemberViewModel
{
    public int Id { get; set; }
    public UserViewModel User { get; set; }
    public ICollection<ChatMemberRoleViewModel> Role { get; set; }
}