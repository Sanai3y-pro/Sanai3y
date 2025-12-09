using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sanaiy.BLL.DTOs.Category
{
    public class UpdateCategoryDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Category Image")]
        public IFormFile? Image { get; set; } // New image file (optional)

        public string? ImagePath { get; set; } // Stored path after upload

        public bool IsActive { get; set; }
    }
}