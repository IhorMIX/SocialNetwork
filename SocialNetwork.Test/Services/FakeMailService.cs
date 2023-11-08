using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;

namespace SocialNetwork.Test.Services;

public class FakeMailService : IMailService
{
    public Task SendHtmlEmailAsync(MailModel mailModel)
    {
        return Task.CompletedTask;
    }
}