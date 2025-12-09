using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Violation
{
    public class ViolationDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; }

        // Reporter Info
        public Guid ReporterUserId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterEmail { get; set; } = string.Empty;
        public string ReporterType { get; set; } = string.Empty;

        // Target User Info
        public Guid? TargetUserId { get; set; }
        public string? TargetUserName { get; set; }
        public string? TargetUserEmail { get; set; }

        // Target Booking Info
        public Guid? TargetBookingId { get; set; }
        public string? ServiceName { get; set; }
        public DateTime? BookingDate { get; set; }
    }
}