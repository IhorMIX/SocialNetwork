using System.Text;

namespace SocialNetwork.BLL.Extensions;

public static class StringExtension
{
    public static string ToBase64(this string str)
    {
        return Convert.ToBase64String(Encoding.Default.GetBytes(str));
    }

    public static string FromBase64ToString(this string base64String)
    {
        return Encoding.Default.GetString(Convert.FromBase64String(base64String));
    }
}