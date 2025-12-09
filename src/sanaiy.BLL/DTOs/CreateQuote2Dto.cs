using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Quote
{
    public class CreateQuote2Dto
    {
        [Required(ErrorMessage = "Booking ID is required")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 999999, ErrorMessage = "Price must be between 1 and 999,999")]
        public decimal Price { get; set; }

        [Range(1, 10080, ErrorMessage = "Duration must be between 1 minute and 7 days (10080 minutes)")]
        public int? Duration { get; set; } // In minutes

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        public int? ExpiryHours { get; set; } = 48; // Default to 48 hours if not provided
    }
}