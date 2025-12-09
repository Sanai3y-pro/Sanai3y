using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Booking
{
    public class BookingListItemDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? PreferredDate { get; set; }
        public string? PreferredTime { get; set; }
        public decimal? PriceFinal { get; set; }
        public DateTime CreatedAt { get; set; }

        // Client Info (Visible to Craftsman)
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientAddressCity { get; set; } // Helpful for craftsman to know the area
        public string? ClientImageUrl { get; set; }
        // Craftsman Info (Visible to Client)
        public string? CraftsmanName { get; set; }
        public decimal? CraftsmanRating { get; set; }
        public string? CraftsmanProfileImageUrl { get; set; }

        public int QuotesCount { get; set; }
        public bool HasReview { get; set; }
        public string? Category { get; set; }
    }
}