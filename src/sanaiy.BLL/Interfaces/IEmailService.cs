using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendWelcomeEmailAsync(string to, string name);
        Task<bool> SendBookingConfirmationAsync(string to, string clientName, string serviceName);
        Task<bool> SendQuoteNotificationAsync(string to, string clientName, decimal price);
        Task<bool> SendPasswordResetEmailAsync(string to, string resetLink);
    }
}