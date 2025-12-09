using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Wallet
{
    public class FreezeWalletDto
    {
        [Required]
        public Guid WalletId { get; set; }

        [Required]
        public bool Freeze { get; set; } // true = Freeze, false = Unfreeze

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}