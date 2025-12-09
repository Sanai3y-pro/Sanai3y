using AutoMapper;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Wallet;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ResultDto<WalletDetailsDto>> GetWalletAsync(Guid craftsmanId)
        {
            try
            {
                var wallets = await _unitOfWork.Wallets.GetWithIncludesAsync(
                    w => w.CraftsmanId == craftsmanId,
                    w => w.Craftsman,
                    w => w.Transactions
                );

                if (!wallets.Any())
                    return new ResultDto<WalletDetailsDto> { Success = false, Message = "Wallet not found" };

                var walletDto = _mapper.Map<WalletDetailsDto>(wallets.First());
                return new ResultDto<WalletDetailsDto> { Success = true, Data = walletDto };
            }
            catch (Exception ex)
            {
                return new ResultDto<WalletDetailsDto> { Success = false, Message = "An error occurred", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<IEnumerable<WalletTransactionDto>>> GetTransactionsAsync(Guid craftsmanId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.CraftsmanId == craftsmanId);
                if (wallet == null)
                    return new ResultDto<IEnumerable<WalletTransactionDto>> { Success = false, Message = "Wallet not found" };

                var (transactions, _) = await _unitOfWork.WalletTransactions.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    t => t.WalletId == wallet.Id,
                    t => t.CreatedAt,
                    ascending: false
                );

                var transactionDtos = _mapper.Map<IEnumerable<WalletTransactionDto>>(transactions);
                return new ResultDto<IEnumerable<WalletTransactionDto>> { Success = true, Data = transactionDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<WalletTransactionDto>> { Success = false, Message = "An error occurred", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<PayoutResponseDto>> RequestPayoutAsync(Guid craftsmanId, RequestPayoutDto payoutDto)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.CraftsmanId == craftsmanId);
                if (wallet == null)
                    return new ResultDto<PayoutResponseDto> { Success = false, Message = "Wallet not found" };

                if (wallet.IsFrozen)
                    return new ResultDto<PayoutResponseDto> { Success = false, Message = "Wallet is frozen" };

                if (wallet.Balance < payoutDto.Amount)
                    return new ResultDto<PayoutResponseDto> { Success = false, Message = "Insufficient balance" };

                wallet.Balance -= payoutDto.Amount;
                _unitOfWork.Wallets.Update(wallet);

                var transaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Type = WalletTransactionType.Payout,
                    Amount = payoutDto.Amount,
                    Direction = TransactionDirection.Out,
                    BalanceAfter = wallet.Balance,
                    Note = payoutDto.Note
                };

                _unitOfWork.WalletTransactions.Add(transaction);
                await _unitOfWork.CompleteAsync();

                var response = new PayoutResponseDto
                {
                    Success = true,
                    Message = "Withdrawal request submitted successfully",
                    TransactionId = transaction.Id,
                    NewBalance = wallet.Balance
                };

                return new ResultDto<PayoutResponseDto> { Success = true, Data = response };
            }
            catch (Exception ex)
            {
                return new ResultDto<PayoutResponseDto> { Success = false, Message = "An error occurred", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> FreezeWalletAsync(FreezeWalletDto freezeDto)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.GetByIdAsync(freezeDto.WalletId);
                if (wallet == null)
                    return new ResultDto { Success = false, Message = "Wallet not found" };

                wallet.IsFrozen = freezeDto.Freeze;
                _unitOfWork.Wallets.Update(wallet);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = freezeDto.Freeze ? "Wallet frozen successfully" : "Wallet unfrozen successfully" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "An error occurred", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> ProcessCommissionAsync(Guid bookingId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
                if (booking == null || booking.CraftsmanId == null || booking.PriceFinal == null)
                    return new ResultDto { Success = false, Message = "Invalid booking" };

                // Calculate commission (e.g., 10%)
                var commissionAmount = booking.PriceFinal.Value * 0.10m;

                var wallet = await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.CraftsmanId == booking.CraftsmanId.Value);
                if (wallet == null)
                    return new ResultDto { Success = false, Message = "Wallet not found" };

                // Deduct commission
                wallet.Balance -= commissionAmount;
                _unitOfWork.Wallets.Update(wallet);

                // Create commission record
                var commission = new Commission
                {
                    BookingId = bookingId,
                    CraftsmanId = booking.CraftsmanId.Value,
                    Amount = commissionAmount,
                    Status = CommissionStatus.Charged,
                    ChargedAt = DateTime.UtcNow
                };
                _unitOfWork.Commissions.Add(commission);

                // Create wallet transaction
                var transaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Type = WalletTransactionType.Commission,
                    Amount = commissionAmount,
                    Direction = TransactionDirection.Out,
                    BalanceAfter = wallet.Balance,
                    RelatedBookingId = bookingId,
                    Note = $"Commission for booking #{bookingId}"
                };
                _unitOfWork.WalletTransactions.Add(transaction);

                await _unitOfWork.CompleteAsync();

                // Link commission to transaction
                commission.WalletTransactionId = transaction.Id;
                _unitOfWork.Commissions.Update(commission);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "Commission deducted successfully" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "An error occurred", Errors = new() { ex.Message } };
            }
        }
    }
}