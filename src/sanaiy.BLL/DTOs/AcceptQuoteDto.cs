using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Booking
{
    public class AcceptQuoteDto
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public Guid QuoteId { get; set; }

        [Required(ErrorMessage = "Payment Method is required")]
        public string PaymentMethod { get; set; } = string.Empty; // "Card" or "Cash"
    }
}