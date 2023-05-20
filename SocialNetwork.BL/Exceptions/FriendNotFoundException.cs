namespace SocialNetwork.BL.Exceptions;

public class FriendNotFoundException: Exception
{
    public FriendNotFoundException(string message) : base(message)
    {
    }
}