using System.Threading.Tasks;

namespace Kodhier.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
