using System.Security.Claims;
using SocialNetwork.Web.Options;

namespace SocialNetwork.Web.Extensions;

public static class AuthorizeExtension
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(AuthOption.AuthOptions.UserIdCalmName);
        
        if (claim == null)
        {
            throw new Exception("Member with id claim didn't exist on identity");
        }

        if (int.TryParse(claim.Value, out var memberId))
        {
            return memberId;
        }

        throw new Exception($"Member id was not an int. Id '{claim.Value}'");
    }
}