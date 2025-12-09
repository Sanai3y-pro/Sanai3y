using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Violation
{
    public class UpdateViolationStatusDto
    {
        [Required]
        public Guid ViolationId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty; // "Investigating", "Resolved", "Rejected"

        [StringLength(500)]
        public string? AdminNote { get; set; }
    }
}