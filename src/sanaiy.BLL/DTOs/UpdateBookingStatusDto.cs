using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Booking
{
    public class UpdateBookingStatusDto
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Note { get; set; }
    }
}