using SocialNetwork.BL.Models;


namespace SocialNetwork.BL.Services.Interfaces
{
    public interface IMailService
    {
        Task SendHtmlEmailAsync(MailModel mailModel);
    }
}
