namespace SocialNetwork.Web.Helpers
{
    public class TemplatePathesHelper
    {
        public static string GetTemplate (string template)
        {
            string filePath = Directory.GetCurrentDirectory() + template; 
            using StreamReader reader = new StreamReader(filePath);
            template = reader.ReadToEnd();
            return template;
        }
    
    }
}
