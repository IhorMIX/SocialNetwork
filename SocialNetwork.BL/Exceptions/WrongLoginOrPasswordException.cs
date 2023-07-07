namespace SocialNetwork.BL.Exceptions;

public class WrongLoginOrPasswordException : CustomException
{
    public WrongLoginOrPasswordException(string message) : base(message)
    {
    }
}