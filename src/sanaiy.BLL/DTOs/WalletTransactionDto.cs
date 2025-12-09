using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Wallet
{
    public class WalletTransactionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // "Deposit", "Commission", "Payout"
        public decimal Amount { get; set; }
        public string Direction { get; set; } = string.Empty; // "In", "Out"
        public decimal BalanceAfter { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // Context
        public Guid? BookingId { get; set; }
        public string? BookingServiceName { get; set; } // Friendly name for the reference
        public string? BookingReference { get; set; }
    }
}