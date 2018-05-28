using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Kodhier.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //Mailbox settings  
            const string fromAddress = "leitbugs.test@gmail.com";
            const string fromAdressTitle = "Kodhier";

            //Smtp Server settings
            const string smtpServer = "smtp.gmail.com";
            const int smtpPortNumber = 587;

            var mimeMessage = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart("html") { Text = message }
            };

            mimeMessage.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
            mimeMessage.To.Add(new MailboxAddress(email, email));

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPortNumber, false);
                client.Authenticate("leitbugs.test@gmail.com", "leitbugs123");
                await client.SendAsync(mimeMessage);
                Console.WriteLine($"An email was sent to {email} with subject: \"{subject}\".");
                await client.DisconnectAsync(true);
            }
        }
    }
}
