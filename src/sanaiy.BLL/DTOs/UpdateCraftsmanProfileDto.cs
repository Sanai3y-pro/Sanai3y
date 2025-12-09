using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sanaiy.BLL.DTOs.Craftsman
{
    public class UpdateCraftsmanProfileDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        public string FName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        // Input: New profile image
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }

        // Stored path after file upload (used by mapping)
        public string? ProfileImagePath { get; set; }
        public string? City { get; set; }
        public string? AvailabilityJson { get; set; }
        public List<CraftsmanAvailabilityDto>? Availability { get; set; }


    }
}