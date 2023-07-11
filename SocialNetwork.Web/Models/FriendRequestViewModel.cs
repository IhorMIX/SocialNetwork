namespace SocialNetwork.Web.Models;

public class FriendRequestViewModel
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public UserViewModel Sender { get; set; }
    public UserViewModel Receiver { get; set; }
}