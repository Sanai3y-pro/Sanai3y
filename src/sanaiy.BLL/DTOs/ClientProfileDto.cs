using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Client
{
    public class ClientProfileDto
    {
        public Guid Id { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Stats for display
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public string? Location { get; set; }
        public List<CraftsmanAvailabilityDto>? CraftsmanAvailability { get; set; }


    }
}