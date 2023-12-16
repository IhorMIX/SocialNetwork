namespace SocialNetwork.BLL.Exceptions;
public class BannedUserException : CustomException
{
    public BannedUserException(string message) : base(message)
    {
    }
}