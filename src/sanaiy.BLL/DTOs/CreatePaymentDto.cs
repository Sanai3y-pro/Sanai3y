using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Booking ID is required")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 999999, ErrorMessage = "Amount must be between 1 and 999,999")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment Method is required")]
        public string Method { get; set; } = string.Empty; // "Card", "Cash", "Wallet"

        // Optional: Reference from Payment Gateway (e.g., Stripe PaymentIntentId)
        public string? ProviderReference { get; set; }
        public string? ProviderName { get; set; }
    }
}
