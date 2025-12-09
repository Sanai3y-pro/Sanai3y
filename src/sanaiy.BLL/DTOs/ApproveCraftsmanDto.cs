using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Craftsman
{
    public class ApproveCraftsmanDto
    {
        [Required]
        public Guid CraftsmanId { get; set; }

        [Required]
        public bool IsApproved { get; set; } // true = Approve, false = Reject

        [StringLength(500)]
        public string? AdminNote { get; set; } // Required if Rejected
    }
}
