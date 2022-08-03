using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tracnghiem.Entities;
using TrueSight.Common;

namespace Tracnghiem.Services.MMail
{
    public interface IMailService : IServiceScoped
    {
        Task<bool> SendEmails(List<Mail> Mails);
    }
    public class MailService : IMailService
    {
        public static string apiKey = "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";
        public static SendGridClient client = new SendGridClient(apiKey);
        public async Task<bool> SendEmails(List<Mail> Mails)
        {
            foreach (var Mail in Mails)
            {
                await Send(Mail);
            }
            return true;
        }

        private async Task Send(Mail Mail)
        {
            var from = new EmailAddress("namtao100@gmail.com", "Admin");
            var subject = Mail.Subject;
            var to = new EmailAddress(Mail.RecipientEmail, Mail.RecipientDisplayName);
            var plainTextContent = "";
            var htmlContent = Mail.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
