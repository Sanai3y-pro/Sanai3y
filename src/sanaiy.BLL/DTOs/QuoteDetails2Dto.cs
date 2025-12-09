using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Quote
{
    public class QuoteDetails2Dto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int? Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsExpired { get; set; }

        // Booking Info
        public Guid BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime? PreferredDate { get; set; }
        public string? PreferredTime { get; set; }
        public string? AdditionalNote { get; set; }

        // Craftsman Info
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public string? CraftsmanPhone { get; set; } // Only visible if accepted
        public decimal CraftsmanRating { get; set; }
        public int? CraftsmanYearsOfExperience { get; set; }
        public string? CraftsmanProfileImageUrl { get; set; }

        // Client Info (Privacy Rules Apply)
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; } // Only visible if accepted
        public string? FullAddress { get; set; } // Only visible if accepted
    }
}