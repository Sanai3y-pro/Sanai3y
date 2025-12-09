using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Review
{
    public class ReviewListItemDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Client Info (Who wrote the review)
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientProfileImageUrl { get; set; }
        public string? ClientProfileImage { get => ClientProfileImageUrl; }

        // Context
        public Guid BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public Guid CraftsmanId { get; set; }
    }
}
