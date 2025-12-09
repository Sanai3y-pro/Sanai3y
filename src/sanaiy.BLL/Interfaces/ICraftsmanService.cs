using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Craftsman;
using sanaiy.BLL.DTOs.Review;
using sanaiy.BLL.DTOs.Service;

public interface ICraftsmanService
{
    Task<ResultDto<CraftsmanProfileDto>> GetProfileAsync(Guid craftsmanId);
    Task<ResultDto> UpdateProfileAsync(Guid craftsmanId, UpdateCraftsmanProfileDto dto);

    // Password
    Task<ResultDto> ChangePasswordAsync(Guid craftsmanId, string currentPassword, string newPassword);

    // Services
    Task<ResultDto<List<ServiceListItemDto>>> GetCraftsmanServicesAsync(Guid craftsmanId);
    Task<ResultDto<ServiceListItemDto>> AddNewServiceAsync(CreateServiceDto dto);

    // Reviews
    Task<ResultDto<IEnumerable<ReviewListItemDto>>> GetCraftsmanReviewsAsync(Guid craftsmanId);

    // Bookings
    Task<ResultDto<List<BookingListItemDto>>> GetCraftsmanBookingsAsync(Guid craftsmanId);


    // Placeholder methods
    Task<ResultDto<IEnumerable<CraftsmanListItemDto>>> GetCraftsmenByCategoryAsync(string category);
    Task<ResultDto<IEnumerable<CraftsmanApplicationDto>>> GetPendingApplicationsAsync();
    Task<ResultDto> ApproveCraftsmanAsync(ApproveCraftsmanDto approveDto);
    Task<ResultDto> RejectCraftsmanAsync(Guid id, string reason);
    Task<ResultDto<PaginatedResultDto<CraftsmanListItemDto>>> SearchCraftsmenAsync(SearchFilterDto filter);


}