using SocialNetwork.BLL.Models;


namespace SocialNetwork.BLL.Services.Interfaces
{
    public interface IMailService
    {
        Task SendHtmlEmailAsync(MailModel mailModel);
    }
}
