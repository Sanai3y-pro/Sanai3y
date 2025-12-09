using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IReviewService
    {
        Task<ResultDto<Guid>> CreateReviewAsync(Guid clientId, CreateReviewDto createDto);
        Task<ResultDto<ReviewDetailsDto>> GetReviewByIdAsync(Guid reviewId);
        Task<ResultDto<IEnumerable<ReviewListItemDto>>> GetCraftsmanReviewsAsync(Guid craftsmanId);
        Task<ResultDto<CraftsmanReviewsSummaryDto>> GetCraftsmanReviewsSummaryAsync(Guid craftsmanId);
        Task<ResultDto> DeleteReviewAsync(Guid reviewId, Guid clientId);
    }
}