using NotificationService.Models;
using System.Net.Mail;

namespace NotificationService.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }

}
