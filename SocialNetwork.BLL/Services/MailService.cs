using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Scriban;
using Scriban.Runtime;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.BLL.Settings;

namespace SocialNetwork.BLL.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettingsOptions _mailSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger, IOptions<MailSettingsOptions> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value; 
        }

        public async Task SendHtmlEmailAsync(MailModel mailModel)
        {
            string mailText = await RenderTemplate(mailModel.Data, mailModel.FilePath); //put html to string
            MimeMessage email = CreateEmailMessage(mailModel.EmailTo, mailModel.Subject); //create mail
            
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = mailText 
            }; 
            
            bodyBuilder = AddAttachmentsHtml(bodyBuilder, mailModel);
            
            email.Body = bodyBuilder.ToMessageBody();
            
            await SendEmailAsync(email); //send
            
            _logger.LogInformation($"Email sent to {mailModel.EmailTo}");
        }

        private BodyBuilder AddAttachmentsHtml(BodyBuilder bodyBuilder, MailModel mailModel)
        {
            if (mailModel.Attachments != null)
            {
                foreach (var file in mailModel.Attachments)
                {
                    if (file.Length > 0) //check if the file length is greater than 0 to ensure it's a valid file. 
                    {
                        var attachment =
                            bodyBuilder.Attachments.Add(file.FileName,
                                file.OpenReadStream()); //create an attachment variable using the Add() method
                        attachment.ContentDisposition =
                            new ContentDisposition //set the disposition of the attachment
                            {
                                Disposition = ContentDisposition.Attachment,
                                FileName = file.FileName,
                                Size = file.Length
                            };
                    }
                }
            }
            
            return bodyBuilder;
        }

        private async Task<string> RenderTemplate(IScriptObject? data, string filePath)
        {
            using StreamReader reader = new StreamReader(filePath);
            
            string mailText = await reader.ReadToEndAsync(); //put in string value html format

            var tpl = Template.Parse(mailText); //string with template

            var context = new TemplateContext //ITemaplteContext
            {
                StrictVariables = true,
                EnableRelaxedIndexerAccess = true,
                EnableRelaxedMemberAccess = true,
                NewLine = "\n"
            };

            if (data != null)
            {
                context.PushGlobal(data); 
            }

            var res = await tpl.RenderAsync(context);
           
            return res;
        }

        private MimeMessage CreateEmailMessage(string emailTo, string subject = "")
        {
            MimeMessage email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject
            };
            
            // sender email
            email.To.Add(MailboxAddress.Parse(emailTo)); // user email
            
            return email;
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port,
                SecureSocketOptions.StartTls); //connect with port and host
            
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password); //verification of sender`s data accuracy
            
            await smtp.SendAsync(email); //send mail genereted in CreateEmailMessage
            
            await smtp.DisconnectAsync(true); //disconect from smtp
        }
    }
}