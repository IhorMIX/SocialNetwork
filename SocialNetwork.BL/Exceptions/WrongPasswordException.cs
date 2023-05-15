namespace SocialNetwork.BL.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException(string message) : base(message)
    {
    }
}