namespace SocialNetwork.BL.Exceptions;

public class RoleNotFoundException : CustomException
{
    public RoleNotFoundException(string message) : base(message)
    {
    }
}