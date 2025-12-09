using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Booking
{
    public class BookingDetailsDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PreferredDate { get; set; }
        public string? PreferredTime { get; set; }
        public string? AdditionalNote { get; set; }
        public decimal? PriceFinal { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }

        // Service Info
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ServiceDescription { get; set; }

        // Client Info
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string? ClientPhone { get; set; }
        public string? ClientProfileImageUrl { get; set; }
        public string? ClientProfileImage { get => ClientProfileImageUrl; }

        // Craftsman Info (Nullable - might not be assigned yet)
        public Guid? CraftsmanId { get; set; }
        public string? CraftsmanName { get; set; }
        public string? CraftsmanPhone { get; set; }
        public decimal? CraftsmanRating { get; set; }
        public string? CraftsmanProfileImageUrl { get; set; }
        public string? CraftsmanProfileImage { get => CraftsmanProfileImageUrl; }

        // Address Info
        public Guid? AddressId { get; set; }
        public string? FullAddress { get; set; }
        public string? City { get; set; }

        // Logic Flags
        public int QuotesCount { get; set; }
        public bool HasAcceptedQuote { get; set; }
        public bool HasPayment { get; set; }
        public bool HasReview { get; set; }

        // Optional: List of Quotes for this booking
        // public List<QuoteDetailsDto> Quotes { get; set; } = new();
    }
}