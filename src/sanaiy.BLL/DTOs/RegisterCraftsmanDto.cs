using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sanaiy.BLL.DTOs.Auth
{
    public class RegisterCraftsmanDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        public string FName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "National ID is required")]
        [StringLength(20)]
        public string NationalID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty; // Will come from Dropdown

        [Required(ErrorMessage = "Years of Experience is required")]
        [Range(0, 50, ErrorMessage = "Years of Experience must be between 0 and 50")]
        public int YearsOfExperience { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        // ==========================================
        // File Uploads (Input from View)
        // ==========================================

        [Display(Name = "National ID Image")]
        public IFormFile? IDCardImage { get; set; }

        [Display(Name = "Drug Test Document")]
        public IFormFile? DrugTestFile { get; set; }

        [Display(Name = "Criminal Record Document")]
        public IFormFile? CriminalRecordFile { get; set; }

        // Stored paths after upload
        public string? IDCardImagePath { get; set; }
        public string? DrugTestFilePath { get; set; }
        public string? CriminalRecordFilePath { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }

    }
}