using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Service
{
    public class UpdateServiceDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Service Name is required")]
        [StringLength(150, ErrorMessage = "Service Name cannot exceed 150 characters")]
        public string ServiceName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        public bool IsPriceFixed { get; set; }

        public bool IsActive { get; set; }
    }
}