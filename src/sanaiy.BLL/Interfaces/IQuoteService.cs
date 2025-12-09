using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IQuoteService
    {
        Task<ResultDto<Guid>> CreateQuoteAsync(Guid craftsmanId, CreateQuoteDto createDto);
        Task<ResultDto<QuoteDetailsDto>> GetQuoteByIdAsync(Guid quoteId);
        Task<ResultDto<IEnumerable<QuoteListItemDto>>> GetBookingQuotesAsync(Guid bookingId);
        Task<ResultDto<IEnumerable<QuoteListItemDto>>> GetCraftsmanQuotesAsync(Guid craftsmanId);
        Task<ResultDto> UpdateQuoteStatusAsync(UpdateQuoteStatusDto updateDto);
        Task<ResultDto> WithdrawQuoteAsync(Guid craftsmanId, Guid quoteId);
    }
}