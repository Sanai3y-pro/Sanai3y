using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Violation
{
    public class ViolationListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Reporter Info (Filled manually in Service)
        public Guid ReporterUserId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterType { get; set; } = string.Empty; // "Client" or "Craftsman"

        // Target Info (Filled manually in Service)
        public Guid? TargetUserId { get; set; }
        public string? TargetUserName { get; set; }

        // Booking Info
        public Guid? TargetBookingId { get; set; }
        public string? BookingServiceName { get; set; }
    }
}
