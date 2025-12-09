using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Service is required")]
        public Guid ServiceId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public Guid AddressId { get; set; }

        [Required(ErrorMessage = "Craftsman is required")]
        public Guid CraftsmanId { get; set; } // ← اضفنا CraftsmanId

        [Required(ErrorMessage = "Preferred Date is required")]
        public DateTime PreferredDate { get; set; }

        [Required(ErrorMessage = "Preferred Time is required")]
        [StringLength(20)]
        public string PreferredTime { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Note cannot exceed 1000 characters")]
        public string? AdditionalNote { get; set; }
    }
}
