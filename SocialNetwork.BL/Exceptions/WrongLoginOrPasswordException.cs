namespace SocialNetwork.BL.Exceptions;

public class WrongLoginOrPasswordException : Exception
{
    public WrongLoginOrPasswordException(string message) : base(message)
    {
    }
}