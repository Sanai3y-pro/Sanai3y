using AutoMapper;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Review;
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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultDto<Guid>> CreateReviewAsync(Guid clientId, CreateReviewDto createDto)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(createDto.BookingId);
                if (booking == null || booking.ClientId != clientId)
                    return new ResultDto<Guid> { Success = false, Message = "الحجز غير موجود" };

                if (booking.Status != BookingStatus.Completed)
                    return new ResultDto<Guid> { Success = false, Message = "لا يمكن التقييم قبل إكمال الحجز" };

                var existingReview = await _unitOfWork.Reviews.SingleOrDefaultAsync(r => r.BookingId == createDto.BookingId);
                if (existingReview != null)
                    return new ResultDto<Guid> { Success = false, Message = "تم التقييم مسبقاً" };

                var review = _mapper.Map<Review>(createDto);
                review.ClientId = clientId;
                review.CraftsmanId = booking.CraftsmanId!.Value;

                _unitOfWork.Reviews.Add(review);
                await _unitOfWork.CompleteAsync();

                // Update craftsman rating
                await UpdateCraftsmanRatingAsync(booking.CraftsmanId.Value);

                return new ResultDto<Guid> { Success = true, Message = "تم إضافة التقييم بنجاح", Data = review.Id };
            }
            catch (Exception ex)
            {
                return new ResultDto<Guid> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<ReviewDetailsDto>> GetReviewByIdAsync(Guid reviewId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetWithIncludesAsync(
                    r => r.Id == reviewId,
                    r => r.Client,
                    r => r.Craftsman,
                    r => r.Booking,
                    r => r.Booking.Service
                );

                if (!reviews.Any())
                    return new ResultDto<ReviewDetailsDto> { Success = false, Message = "التقييم غير موجود" };

                var reviewDto = _mapper.Map<ReviewDetailsDto>(reviews.First());
                return new ResultDto<ReviewDetailsDto> { Success = true, Data = reviewDto };
            }
            catch (Exception ex)
            {
                return new ResultDto<ReviewDetailsDto> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<IEnumerable<ReviewListItemDto>>> GetCraftsmanReviewsAsync(Guid craftsmanId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetWithIncludesAsync(
                    r => r.CraftsmanId == craftsmanId,
                    r => r.Client,
                    r => r.Booking,
                    r => r.Booking.Service
                );

                var reviewDtos = _mapper.Map<IEnumerable<ReviewListItemDto>>(reviews.OrderByDescending(r => r.CreatedAt));
                return new ResultDto<IEnumerable<ReviewListItemDto>> { Success = true, Data = reviewDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<ReviewListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<CraftsmanReviewsSummaryDto>> GetCraftsmanReviewsSummaryAsync(Guid craftsmanId)
        {
            try
            {
                var craftsman = await _unitOfWork.Craftsmen.GetWithIncludesAsync(
                    c => c.Id == craftsmanId,
                    c => c.ReviewsReceived
                );

                if (!craftsman.Any())
                    return new ResultDto<CraftsmanReviewsSummaryDto> { Success = false, Message = "الحرفي غير موجود" };

                var craftsmanEntity = craftsman.First();
                var reviews = craftsmanEntity.ReviewsReceived.ToList();

                var summary = new CraftsmanReviewsSummaryDto
                {
                    CraftsmanId = craftsmanId,
                    CraftsmanName = craftsmanEntity.FullName,
                    AverageRating = craftsmanEntity.RatingAverage,
                    TotalReviews = reviews.Count,
                    FiveStars = reviews.Count(r => r.Rating == 5),
                    FourStars = reviews.Count(r => r.Rating == 4),
                    ThreeStars = reviews.Count(r => r.Rating == 3),
                    TwoStars = reviews.Count(r => r.Rating == 2),
                    OneStar = reviews.Count(r => r.Rating == 1)
                };

                return new ResultDto<CraftsmanReviewsSummaryDto> { Success = true, Data = summary };
            }
            catch (Exception ex)
            {
                return new ResultDto<CraftsmanReviewsSummaryDto> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> DeleteReviewAsync(Guid reviewId, Guid clientId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null || review.ClientId != clientId)
                    return new ResultDto { Success = false, Message = "التقييم غير موجود" };

                var craftsmanId = review.CraftsmanId;

                _unitOfWork.Reviews.Remove(review);
                await _unitOfWork.CompleteAsync();

                // Update craftsman rating
                await UpdateCraftsmanRatingAsync(craftsmanId);

                return new ResultDto { Success = true, Message = "تم حذف التقييم" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        private async Task UpdateCraftsmanRatingAsync(Guid craftsmanId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.CraftsmanId == craftsmanId);
            var reviewsList = reviews.ToList();

            if (reviewsList.Any())
            {
                var avgRating = reviewsList.Average(r => r.Rating);
                var craftsman = await _unitOfWork.Craftsmen.GetByIdAsync(craftsmanId);
                if (craftsman != null)
                {
                    craftsman.RatingAverage = Math.Round((decimal)avgRating, 2);
                    _unitOfWork.Craftsmen.Update(craftsman);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }

}
