using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Kodhier.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //From Address  
            const string fromAddress = "leitbugs.test@gmail.com";
            const string fromAdressTitle = "Kodhier";
            //To Address  
            var toAddress = email;
            const string toAdressTitle = "Sveiki";
            var bodyContent = message;

            //Smtp Server  
            const string smtpServer = "smtp.gmail.com";
            //Smtp Port Number  
            const int smtpPortNumber = 587;

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress
            (fromAdressTitle,
                fromAddress
            ));
            mimeMessage.To.Add(new MailboxAddress
            (toAdressTitle,
                toAddress
            ));
            mimeMessage.Subject = subject; //Subject
            mimeMessage.Body = new TextPart("html")
            {
                Text = bodyContent
            };
            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPortNumber, false);
                client.Authenticate("leitbugs.test@gmail.com", "leitbugs123");
                await client.SendAsync(mimeMessage);
                Console.WriteLine("The mail has been sent successfully !!");
                //Console.ReadLine();
                await client.DisconnectAsync(true);
            }
        }
    }
}
