namespace SocialNetwork.BL.Exceptions;

public class FriendNotFoundException: CustomException
{
    public FriendNotFoundException(string message) : base(message)
    {
    }
}