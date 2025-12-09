using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<ResultDto<Guid>> CreateBookingAsync(Guid clientId, CreateBookingDto createDto);
        Task<ResultDto<BookingDetailsDto>> GetBookingByIdAsync(Guid bookingId);
        Task<ResultDto<IEnumerable<BookingListItemDto>>> GetClientBookingsAsync(Guid clientId);
        Task<ResultDto<List<BookingListItemDto>>> GetCraftsmanBookingsAsync(Guid craftsmanId);
        Task<ResultDto> UpdateBookingStatusAsync(UpdateBookingStatusDto updateDto);
        Task<ResultDto> CancelBookingAsync(Guid bookingId, Guid userId);
        Task<ResultDto> AcceptQuoteAsync(Guid clientId, AcceptQuoteDto acceptDto);
        Task<ResultDto> CompleteBookingAsync(Guid bookingId);
        Task<ResultDto<PaginatedResultDto<BookingListItemDto>>> GetBookingsPaginatedAsync(int pageNumber, int pageSize, string? status = null);

    }
}