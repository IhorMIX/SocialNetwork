using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialNetwork.Web.Options;

public class AuthOption
{
    public class AuthOptions
    {
        public const string ISSUER = "FreeSocialNetwork"; // token publisher
        public const string AUDIENCE = "SocialNetwork"; // token customer
        const string KEY = "mysupersecret_secretkey!123";   // encryption key
        public const int LIFETIME = 1440; // token liftime
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}