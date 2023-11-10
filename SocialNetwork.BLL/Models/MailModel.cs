using Microsoft.AspNetCore.Http;
using Scriban.Runtime;

namespace SocialNetwork.BLL.Models
{
    public class MailModel
    {
        public IScriptObject? Data { get; set; } 

        public string FilePath { get; set; } = null!;

        public string Subject { get; set; } = "";

        public string EmailTo { get; set; } = null!;
        
        public List<IFormFile>? Attachments { get; set; }
    }
}