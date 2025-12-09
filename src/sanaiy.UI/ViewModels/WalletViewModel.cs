using sanaiy.BLL.DTOs.Wallet;

public class WalletViewModel
{
    public WalletDetailsDto Wallet { get; set; }
    public IEnumerable<WalletTransactionDto> Transactions { get; set; }
    public RequestPayoutDto PayoutRequest { get; set; }
}
