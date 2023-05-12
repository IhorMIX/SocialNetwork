using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Web.Options;

namespace SocialNetwork.Web.Helpers;

public class TokenHelper
{
    public string GetToken(int userId)
    {
        var identity = GetIdentity(userId);
        
        var now = DateTime.UtcNow;
        
        var jwt = new JwtSecurityToken(
            issuer: AuthOption.AuthOptions.ISSUER,
            audience: AuthOption.AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOption.AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOption.AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    private ClaimsIdentity GetIdentity(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString()),
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        
        return claimsIdentity;
    }
}