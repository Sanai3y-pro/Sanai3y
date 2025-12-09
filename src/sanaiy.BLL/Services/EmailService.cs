using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            // TODO: Implement actual email sending using SMTP or SendGrid
            await Task.Delay(100); // Simulate async operation
            Console.WriteLine($"Email sent to {to}: {subject}");
            return true;
        }

        public async Task<bool> SendWelcomeEmailAsync(string to, string name)
        {
            var subject = "مرحباً بك في صنايعي";
            var body = $@"
                <h2>مرحباً {name}</h2>
                <p>نشكرك على التسجيل في منصة صنايعي</p>
                <p>يمكنك الآن تصفح الخدمات والحجز</p>
            ";
            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendBookingConfirmationAsync(string to, string clientName, string serviceName)
        {
            var subject = "تأكيد الحجز";
            var body = $@"
                <h2>عزيزي {clientName}</h2>
                <p>تم تأكيد حجزك لخدمة: {serviceName}</p>
                <p>سيتم إرسال عروض الأسعار قريباً</p>
            ";
            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendQuoteNotificationAsync(string to, string clientName, decimal price)
        {
            var subject = "عرض سعر جديد";
            var body = $@"
                <h2>عزيزي {clientName}</h2>
                <p>لديك عرض سعر جديد بقيمة {price} جنيه</p>
                <p>يمكنك مراجعة العرض وقبوله من حسابك</p>
            ";
            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "إعادة تعيين كلمة المرور";
            var body = $@"
                <h2>إعادة تعيين كلمة المرور</h2>
                <p>اضغط على الرابط لإعادة تعيين كلمة المرور:</p>
                <a href='{resetLink}'>إعادة تعيين كلمة المرور</a>
            ";
            return await SendEmailAsync(to, subject, body);
        }
    }
}
