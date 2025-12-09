using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Quote
{
    public class CreateQuoteDto
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 100000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(1, 365, ErrorMessage = "Duration must be at least 1 day")]
        public int? Duration { get; set; } // e.g., in Days or Hours

        [StringLength(500)]
        public string? Note { get; set; }

        // Optional: expiry hours for the quote
        public int? ExpiryHours { get; set; }
    }
}
