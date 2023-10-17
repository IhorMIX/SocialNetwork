namespace SocialNetwork.BL.Exceptions;
public class BanYourselfException : CustomException
{
    public BanYourselfException(string message) : base(message)
    {
    }
}