namespace SocialNetwork.BL.Exceptions;

public class AlreadyLoginAndEmailException : Exception
{
    public AlreadyLoginAndEmailException(string message) : base(message)
    {

    }
}
