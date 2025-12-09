using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface INotificationService
    {
        Task NotifyNewBookingAsync(Guid craftsmanId, Guid bookingId);
        Task NotifyQuoteReceivedAsync(Guid clientId, Guid quoteId);
        Task NotifyQuoteAcceptedAsync(Guid craftsmanId, Guid quoteId);
        Task NotifyBookingCompletedAsync(Guid clientId, Guid bookingId);
        Task NotifyPaymentReceivedAsync(Guid craftsmanId, decimal amount);
    }
}
