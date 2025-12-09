using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Service
{
    public class CreateServiceDto
    {
        [Required(ErrorMessage = "Service Name is required")]
        [StringLength(150, ErrorMessage = "Service Name cannot exceed 150 characters")]
        public string ServiceName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required]
        public bool IsPriceFixed { get; set; }

        [Required]
        public Guid CraftsmanId { get; set; }
    }
}
