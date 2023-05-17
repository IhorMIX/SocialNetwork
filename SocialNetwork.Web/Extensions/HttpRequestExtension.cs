using System.Text;

namespace SocialNetwork.Web.Extensions;

public static class HttpRequestExtension
{
    public static async Task<string> GetRequestBodyAsStringAsync(this HttpRequest request)
    {
        using (var streamReader = new StreamReader(request.Body, Encoding.UTF8))
        {
            return await streamReader.ReadToEndAsync();
        }
    }
}