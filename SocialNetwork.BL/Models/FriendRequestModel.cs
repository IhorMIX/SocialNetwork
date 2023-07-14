namespace SocialNetwork.BL.Models;

public class FriendRequestModel
{
        
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public UserModel Sender { get; set; }
        public UserModel Receiver { get; set; }
}