using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Address
{
    public class CreateAddressDto
    {
        [Required(ErrorMessage = "Full Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Type is required")]
        public string AddressType { get; set; } = string.Empty; // "Home", "Work", "Other"

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        // Geolocation (Optional but recommended for Maps)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}