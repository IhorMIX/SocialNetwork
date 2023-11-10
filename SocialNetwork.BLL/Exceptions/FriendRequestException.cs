namespace SocialNetwork.BLL.Exceptions;

public class FriendRequestException : CustomException
{
    public FriendRequestException(string message) : base(message)
    {
    }
}