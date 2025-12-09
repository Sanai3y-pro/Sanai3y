using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Wallet
{
    public class WalletDetailsDto
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public bool IsFrozen { get; set; }
        public DateTime CreatedAt { get; set; }

        // Craftsman Info
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;

        // Calculated Statistics
        public int TotalTransactions { get; set; }
        public decimal TotalEarnings { get; set; }     // Total Deposits
        public decimal TotalCommissions { get; set; }  // Total Paid Commissions
        public decimal TotalPayouts { get; set; }      // Total Withdrawn
    }
}
