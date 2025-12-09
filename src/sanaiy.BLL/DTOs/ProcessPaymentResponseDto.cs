using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Payment
{
    public class ProcessPaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? PaymentId { get; set; }
        public string? TransactionReference { get; set; }
    }
}