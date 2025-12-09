using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Service
{
    public class ServiceDetailsDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPriceFixed { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Category Info
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Craftsman Info (Detailed)
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public string? CraftsmanProfileImage { get; set; }
        public decimal CraftsmanRating { get; set; }
        public int CraftsmanYearsOfExperience { get; set; }
        public string? CraftsmanBio { get; set; }

        // Statistics
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
    }
}