namespace SocialNetwork.Web.Models;

public class FriendRequestViewModel
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public FriendViewModel Sender { get; set; }
    public FriendViewModel Receiver { get; set; }
}