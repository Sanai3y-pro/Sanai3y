using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Craftsman
{
    public class CraftsmanProfileDto
    {
        public Guid Id { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? NationalID { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public string? Category { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Bio { get; set; }

        // Stats
        public decimal RatingAverage { get; set; }
        public int TotalReviews { get; set; }
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }

        // Financial (Private Info)
        public decimal WalletBalance { get; set; }

        // Images
        public string? ProfileImageUrl { get; set; }
        public string? IDCardImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? City { get; set; }
        public List<CraftsmanAvailabilityDto>? CraftsmanAvailability { get; set; }


    }
}
