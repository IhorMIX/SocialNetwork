namespace SocialNetwork.BLL.Exceptions;

public class ChatNotFoundException : CustomException
{
    public ChatNotFoundException(string message) : base(message)
    {
    }
}