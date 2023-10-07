namespace SocialNetwork.BL.Exceptions;

public class NoRightException: CustomException
{
    public NoRightException(string message) : base(message)
    {
    }
}