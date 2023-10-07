using Scriban.Runtime;
using SocialNetwork.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BL.Services.Interfaces
{
    public interface IMailService
    {
        Task SendHTMLEmailAsync(IScriptObject test, string template);
    }
}
