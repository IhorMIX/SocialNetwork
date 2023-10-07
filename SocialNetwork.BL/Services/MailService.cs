using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Scriban;
using Scriban.Runtime;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Settings;
using SocialNetwork.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BL.Services
{
    internal class MailService
    {
        private readonly MailSettingsModel _mailSettings;//model bl.models
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public MailService( ILogger<UserService> logger, IMapper mapper, IOptions<MailSettingsModel> mailSettings)
        {
            _logger = logger;
            _mapper = mapper;
            _mailSettings = mailSettings.Value;//model bl.models
        }
        public async Task SendHTMLEmailAsync(UserModel request)
        {
            var test = new ScriptObject();
            test.Add("name");
            string mailText = ReadEmailTemplate(request); //put html to string
                                                          // string formattedMailText = FormatEmailText(mailText, request); //replace user data
            MimeMessage email = CreateEmailMessage(request); //create mail
            BodyBuilder bodyBuilder = new BodyBuilder(); // CreateAttachmentsHTML(request); for using attachments
            bodyBuilder.HtmlBody = mailText; //created html body with formattedMailText
            email.Body = bodyBuilder.ToMessageBody();
            await SendEmailAsync(email); //send
        }
        private BodyBuilder CreateAttachmentsHTML(MailModel attacmentsRequest)
        {
            var builder = new BodyBuilder();

            if (attacmentsRequest.Attachments != null)
            {
                foreach (var file in attacmentsRequest.Attachments)
                {
                    if (file.Length > 0) //check if the file length is greater than 0 to ensure it's a valid file. 
                    {
                        var attachment = builder.Attachments.Add(file.FileName, file.OpenReadStream()); //create an attachment variable using the Add() method
                        attachment.ContentDisposition = new MimeKit.ContentDisposition //set the disposition of the attachment
                        {
                            Disposition = MimeKit.ContentDisposition.Attachment,
                            FileName = file.FileName,
                            Size = file.Length
                        };
                    }
                }
            }

            //builder.HtmlBody = mailRequest.Body;
            return builder;
        }

        private string ReadEmailTemplate(UserModel request)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\Welcome.html"; //location of the html template file
            using StreamReader reader = new StreamReader(filePath);
            string mailText = reader.ReadToEnd(); //put in string value html format

            var tpl = Template.Parse(mailText);//string with template

            var context = new TemplateContext //ITemaplteContext
            {
                
            };

            context.PushGlobal(); //variable IScriptObject

            var res = tpl.Render(new { username = request.Profile.Name, email = request.Profile.Email, link=request.Id });
            //$"{HttpContext.Current.RequestUrl.Scheme}://{HttpContext.Current.Request.Url.Authority}/api/User/activation/{specialKeyForActivation}"
            return res;
        }

        //private string FormatEmailText(string mailText, UserModel request)
        //{
        //    string formattedText = mailText.Replace("[username]", request.Profile.Name).Replace("[email]", request.Profile.Email);
        //    return formattedText; //replace user data (name and email)
        //}

        private MimeMessage CreateEmailMessage(UserModel request)
        {
            MimeMessage email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail); // sender email
            email.To.Add(MailboxAddress.Parse(request.Profile.Email)); // user email
            email.Subject = $"Welcome {request.Profile.Name}"; //title
            return email;
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls); //connect with port and host
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password); //verification of sender`s data accuracy
            await smtp.SendAsync(email); //send mail genereted in CreateEmailMessage
            smtp.Disconnect(true); //disconect from smtp
        }
    }
}
