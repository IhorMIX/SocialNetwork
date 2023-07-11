namespace SocialNetwork.BL.Exceptions;

public class FriendRequestNotFoundException : CustomException
{
    public FriendRequestNotFoundException(string message) : base(message)
    {
    }
}