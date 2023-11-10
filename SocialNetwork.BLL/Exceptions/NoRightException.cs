namespace SocialNetwork.BLL.Exceptions;

public class NoRightException: CustomException
{
    public NoRightException(string message) : base(message)
    {
    }
}