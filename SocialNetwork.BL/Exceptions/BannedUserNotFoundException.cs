namespace SocialNetwork.BL.Exceptions;
public class BannedUserNotFoundException : CustomException
{
    public BannedUserNotFoundException(string message) : base(message)
    {
    }
}