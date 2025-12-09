using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Wallet
{
    public class RequestPayoutDto
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 999999, ErrorMessage = "Amount must be between 1 and 999,999")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }
}