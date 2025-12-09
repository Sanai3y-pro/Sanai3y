using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Payment
{
    public class PaymentDetailsDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Captured, Failed
        public DateTime Date { get; set; }
        public string? ProviderReference { get; set; }
        public string? ProviderName { get; set; }

        // Context Info
        public Guid BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? CraftsmanName { get; set; }
    }
}