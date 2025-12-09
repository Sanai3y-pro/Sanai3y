using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Review
{
    public class ReviewDetailsDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Client Info
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientProfileImageUrl { get; set; }
        public string? ClientProfileImage { get => ClientProfileImageUrl; }

        // Craftsman Info
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;

        // Booking Info
        public Guid BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public DateTime? BookingDate { get; set; }
    }
}