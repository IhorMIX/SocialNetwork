namespace SocialNetwork.Web.Models;

public class ChatMemberViewModel
{
    public int Id { get; set; }
    public UserViewModel User { get; set; } = null!;
    public ICollection<ChatMemberRoleViewModel> Role { get; set; } = null!;
}