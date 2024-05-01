using SocialNetwork.BLL.Models;

namespace SocialNetwork.Web.Models;

public class GroupRequestViewModel
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public UserViewModel Sender { get; set; } = null!;
    public GroupViewModel Receiver { get; set; } = null!;
}