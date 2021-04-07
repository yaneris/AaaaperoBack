using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Services
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
                // you will need your own email address here which has been added in sendgrid as an authorized sender
                From = new EmailAddress("your email", "Dorset College"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            foreach(var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Response response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}