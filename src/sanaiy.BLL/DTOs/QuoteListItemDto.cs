using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Quote
{
    public class QuoteListItemDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int? Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsExpired { get; set; }

        // Craftsman Info (Essential for Client decision)
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public decimal CraftsmanRating { get; set; }
        public int CraftsmanYearsOfExperience { get; set; }
        public string? CraftsmanProfileImage { get; set; }

        // Booking Context
        public Guid BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}