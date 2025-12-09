using AutoMapper;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Quote;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public QuoteService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ResultDto<Guid>> CreateQuoteAsync(Guid craftsmanId, CreateQuoteDto createDto)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(createDto.BookingId);
                if (booking == null || booking.Status != BookingStatus.Pending)
                    return new ResultDto<Guid> { Success = false, Message = "الحجز غير متاح" };

                var quote = _mapper.Map<Quote>(createDto);
                quote.CraftsmanId = craftsmanId;
                quote.Status = QuoteStatus.Sent;
                quote.ExpiresAt = DateTime.UtcNow.AddHours(createDto.ExpiryHours ?? 48);

                _unitOfWork.Quotes.Add(quote);
                await _unitOfWork.CompleteAsync();

                booking.Status = BookingStatus.Offered;
                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.CompleteAsync();

                await _notificationService.NotifyQuoteReceivedAsync(booking.ClientId, quote.Id);

                return new ResultDto<Guid> { Success = true, Message = "تم إرسال العرض بنجاح", Data = quote.Id };
            }
            catch (Exception ex)
            {
                return new ResultDto<Guid> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<QuoteDetailsDto>> GetQuoteByIdAsync(Guid quoteId)
        {
            try
            {
                var quotes = await _unitOfWork.Quotes.GetWithIncludesAsync(
                    q => q.Id == quoteId,
                    q => q.Booking,
                    q => q.Booking.Service,
                    q => q.Craftsman,
                    q => q.Booking.Client
                );

                if (!quotes.Any())
                    return new ResultDto<QuoteDetailsDto> { Success = false, Message = "العرض غير موجود" };

                var quoteDto = _mapper.Map<QuoteDetailsDto>(quotes.First());
                return new ResultDto<QuoteDetailsDto> { Success = true, Data = quoteDto };
            }
            catch (Exception ex)
            {
                return new ResultDto<QuoteDetailsDto> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<IEnumerable<QuoteListItemDto>>> GetBookingQuotesAsync(Guid bookingId)
        {
            try
            {
                var quotes = await _unitOfWork.Quotes.GetWithIncludesAsync(
                    q => q.BookingId == bookingId,
                    q => q.Craftsman,
                    q => q.Booking,
                    q => q.Booking.Service
                );

                var quoteDtos = _mapper.Map<IEnumerable<QuoteListItemDto>>(quotes.OrderByDescending(q => q.CreatedAt));
                return new ResultDto<IEnumerable<QuoteListItemDto>> { Success = true, Data = quoteDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<QuoteListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<IEnumerable<QuoteListItemDto>>> GetCraftsmanQuotesAsync(Guid craftsmanId)
        {
            try
            {
                var quotes = await _unitOfWork.Quotes.GetWithIncludesAsync(
                    q => q.CraftsmanId == craftsmanId,
                    q => q.Booking,
                    q => q.Booking.Service
                );

                var quoteDtos = _mapper.Map<IEnumerable<QuoteListItemDto>>(quotes.OrderByDescending(q => q.CreatedAt));
                return new ResultDto<IEnumerable<QuoteListItemDto>> { Success = true, Data = quoteDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<QuoteListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> UpdateQuoteStatusAsync(UpdateQuoteStatusDto updateDto)
        {
            try
            {
                var quote = await _unitOfWork.Quotes.GetByIdAsync(updateDto.QuoteId);
                if (quote == null)
                    return new ResultDto { Success = false, Message = "العرض غير موجود" };

                if (Enum.TryParse<QuoteStatus>(updateDto.Status, out var newStatus))
                {
                    quote.Status = newStatus;
                    _unitOfWork.Quotes.Update(quote);
                    await _unitOfWork.CompleteAsync();

                    return new ResultDto { Success = true, Message = "تم تحديث حالة العرض" };
                }

                return new ResultDto { Success = false, Message = "الحالة غير صحيحة" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> WithdrawQuoteAsync(Guid craftsmanId, Guid quoteId)
        {
            try
            {
                var quote = await _unitOfWork.Quotes.GetByIdAsync(quoteId);
                if (quote == null || quote.CraftsmanId != craftsmanId)
                    return new ResultDto { Success = false, Message = "العرض غير موجود" };

                if (quote.Status != QuoteStatus.Sent)
                    return new ResultDto { Success = false, Message = "لا يمكن سحب هذا العرض" };

                _unitOfWork.Quotes.Remove(quote);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم سحب العرض" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }
    }

}
