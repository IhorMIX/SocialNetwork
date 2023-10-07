namespace SocialNetwork.BL.Exceptions;

public class ChatNotFoundException : CustomException
{
    public ChatNotFoundException(string message) : base(message)
    {
    }
}