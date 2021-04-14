using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AaaaperoBack.Services
{
    public interface IEmailService
    {
        Task<Response> SendEmailAsync(List<string> emails, string subject, string message);
    }
    public class EmailService : IEmailService
    {
        public IConfiguration Configuration {get;}

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<Response> SendEmailAsync(List<string> emails, string subject, string message)
        {
            return await ExecuteEmail(Configuration["SendgridKey"], subject, message, emails);
        }

        public async Task<Response> ExecuteEmail(string apiKey, string subject, string message, List<string> emails)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                // my email address that had been added in sendgrid as an authorized sender
                
                From = new EmailAddress("viteltoosus@hotmail.com", "Aaaapero Team"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = File.ReadAllText("Services/emailDesign.html")
            };

            msg.AddSubstitution("{{Token}}", message);

            foreach(var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Response response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}