namespace SocialNetwork.BL.Exceptions;

public class FriendRequestException : CustomException
{
    public FriendRequestException(string message) : base(message)
    {
    }
}