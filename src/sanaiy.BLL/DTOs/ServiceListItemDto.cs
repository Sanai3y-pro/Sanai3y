using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Service
{
    public class ServiceListItemDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPriceFixed { get; set; }
        public bool IsActive { get; set; }

        // Category Info
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Craftsman Info (Essential for listing)
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public decimal CraftsmanRating { get; set; }
        public string? CraftsmanProfileImageUrl { get; set; } // Added for better UI
    }
}
