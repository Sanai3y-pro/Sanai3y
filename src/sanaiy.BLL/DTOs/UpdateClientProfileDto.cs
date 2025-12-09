using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sanaiy.BLL.DTOs.Client
{
    public class UpdateClientProfileDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters")]
        public string FName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters")]
        public string LName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string? Phone { get; set; }

        // Input: Image file from the form
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }

        // Stored path after file upload (used by mapping)
        public string? ProfileImagePath { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }


    }
}