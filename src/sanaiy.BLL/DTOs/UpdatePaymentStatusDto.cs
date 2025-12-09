using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Payment
{
    public class UpdatePaymentStatusDto
    {
        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; // "Captured", "Failed", "Refunded"

        public string? ProviderReference { get; set; }
    }
}