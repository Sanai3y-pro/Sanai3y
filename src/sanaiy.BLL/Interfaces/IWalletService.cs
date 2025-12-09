using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IWalletService
    {
        Task<ResultDto<WalletDetailsDto>> GetWalletAsync(Guid craftsmanId);
        Task<ResultDto<IEnumerable<WalletTransactionDto>>> GetTransactionsAsync(Guid craftsmanId, int pageNumber = 1, int pageSize = 20);
        Task<ResultDto<PayoutResponseDto>> RequestPayoutAsync(Guid craftsmanId, RequestPayoutDto payoutDto);
        Task<ResultDto> FreezeWalletAsync(FreezeWalletDto freezeDto);
        Task<ResultDto> ProcessCommissionAsync(Guid bookingId);
    }
}
