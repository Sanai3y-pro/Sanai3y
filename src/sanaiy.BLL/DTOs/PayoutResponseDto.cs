using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Wallet
{
    public class PayoutResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? TransactionId { get; set; }
        public decimal? NewBalance { get; set; }
    }
}