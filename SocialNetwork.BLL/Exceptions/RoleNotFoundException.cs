namespace SocialNetwork.BLL.Exceptions;

public class RoleNotFoundException : CustomException
{
    public RoleNotFoundException(string message) : base(message)
    {
    }
}