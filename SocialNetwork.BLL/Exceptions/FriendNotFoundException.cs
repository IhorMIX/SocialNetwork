namespace SocialNetwork.BLL.Exceptions;

public class FriendNotFoundException: CustomException
{
    public FriendNotFoundException(string message) : base(message)
    {
    }
}