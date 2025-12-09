using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;

namespace sanaiy.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper,
            INotificationService notificationService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public async Task<ResultDto<Guid>> CreateBookingAsync(Guid clientId, CreateBookingDto createDto)
        {
            try
            {
                // التحقق من وجود الخدمة
                var service = await _unitOfWork.Services.GetByIdAsync(createDto.ServiceId);
                if (service == null || !service.IsActive)
                    return new ResultDto<Guid> { Success = false, Message = "الخدمة غير موجودة أو غير نشطة" };

                // التحقق من العنوان
                var address = await _unitOfWork.Addresses.GetByIdAsync(createDto.AddressId);
                if (address == null || address.ClientId != clientId)
                    return new ResultDto<Guid> { Success = false, Message = "العنوان غير صحيح" };

                var booking = _mapper.Map<Booking>(createDto);
                booking.ClientId = clientId;
                booking.Status = BookingStatus.Pending;
                booking.IsActive = true;
                booking.ServicesName = service.ServiceName;

                _unitOfWork.Bookings.Add(booking);
                await _unitOfWork.CompleteAsync();

                // إشعار صاحب الخدمة
                await _notificationService.NotifyNewBookingAsync(service.OwnerCraftsmanId, booking.Id);

                // إرسال تأكيد عبر البريد الإلكتروني
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                await _emailService.SendBookingConfirmationAsync(client!.Email, client.FullName, service.ServiceName);

                return new ResultDto<Guid> { Success = true, Message = "تم إنشاء الحجز بنجاح", Data = booking.Id };
            }
            catch (Exception ex)
            {
                return new ResultDto<Guid> { Success = false, Message = "حدث خطأ أثناء إنشاء الحجز", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<BookingDetailsDto>> GetBookingByIdAsync(Guid bookingId)
        {
            try
            {
                var bookings = await _unitOfWork.Bookings.GetWithIncludesAsync(
                    b => b.Id == bookingId,
                    b => b.Service,
                    b => b.Client,
                    b => b.Craftsman,
                    b => b.Location,
                    b => b.Quotes,
                    b => b.Review,
                    b => b.PaymentTransaction
                );

                if (!bookings.Any())
                    return new ResultDto<BookingDetailsDto> { Success = false, Message = "الحجز غير موجود" };

                var booking = bookings.First();
                var detailsDto = _mapper.Map<BookingDetailsDto>(booking);

                return new ResultDto<BookingDetailsDto> { Success = true, Data = detailsDto };
            }
            catch (Exception ex)
            {
                return new ResultDto<BookingDetailsDto> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<IEnumerable<BookingListItemDto>>> GetClientBookingsAsync(Guid clientId)
        {
            try
            {
                var bookings = await _unitOfWork.Bookings.GetWithIncludesAsync(
                    b => b.ClientId == clientId && b.IsActive,
                    b => b.Service,
                    b => b.Craftsman,
                    b => b.Quotes,
                    b => b.Review
                );

                var bookingDtos = _mapper.Map<IEnumerable<BookingListItemDto>>(bookings.OrderByDescending(b => b.CreatedAt));

                return new ResultDto<IEnumerable<BookingListItemDto>> { Success = true, Data = bookingDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<BookingListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }
        public async Task<ResultDto<List<BookingListItemDto>>> GetCraftsmanBookingsAsync(Guid craftsmanId)
        {
            try
            {
                var services = await _unitOfWork.Services.FindAsync(s => s.OwnerCraftsmanId == craftsmanId);
                var serviceIds = services.Select(s => s.Id).ToList();

                var bookings = await _unitOfWork.Bookings.GetWithIncludesAsync(
                    b => (b.CraftsmanId == craftsmanId || serviceIds.Contains(b.ServiceId)) && b.IsActive,
                    b => b.Service,
                    b => b.Client,
                    b => b.Quotes,
                    b => b.Review,
                    b => b.Service.Category
                );

                // هنا بنعمل Map وناخد الكاتيجوري من Service
                var bookingDtos = bookings.Select(b => new BookingListItemDto
                {
                    Id = b.Id,
                    ServiceName = b.Service?.ServiceName ?? "",
                    Status = b.Status.ToString(),
                    PreferredDate = b.PreferredDate,
                    PreferredTime = b.PreferredTime,
                    PriceFinal = b.PriceFinal,
                    CreatedAt = b.CreatedAt,

                    // Client info
                    ClientName = b.Client?.FullName,
                    ClientImageUrl = b.Client?.ProfileImageUrl,

                    // Craftsman info: ممكن تكون مباشرة من Craftsman navigation property
                    CraftsmanName = b.Craftsman?.FullName ?? b.Service?.OwnerCraftsman?.FullName,
                    CraftsmanProfileImageUrl = b.Craftsman?.ProfileImageUrl ?? b.Service?.OwnerCraftsman?.ProfileImageUrl,

                    // Category من الخدمة
                    Category = b.Service?.Category?.Name
                }).ToList();


                return new ResultDto<List<BookingListItemDto>> { Success = true, Data = bookingDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<List<BookingListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }


        public async Task<ResultDto> UpdateBookingStatusAsync(UpdateBookingStatusDto updateDto)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(updateDto.BookingId);
                if (booking == null)
                    return new ResultDto { Success = false, Message = "الحجز غير موجود" };

                if (Enum.TryParse<BookingStatus>(updateDto.Status, out var newStatus))
                {
                    booking.Status = newStatus;
                    _unitOfWork.Bookings.Update(booking);
                    await _unitOfWork.CompleteAsync();

                    return new ResultDto { Success = true, Message = "تم تحديث حالة الحجز" };
                }

                return new ResultDto { Success = false, Message = "الحالة غير صحيحة" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> CancelBookingAsync(Guid bookingId, Guid userId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
                if (booking == null)
                    return new ResultDto { Success = false, Message = "الحجز غير موجود" };

                if (booking.ClientId != userId && booking.CraftsmanId != userId)
                    return new ResultDto { Success = false, Message = "ليس لديك صلاحية إلغاء هذا الحجز" };

                if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                    return new ResultDto { Success = false, Message = "لا يمكن إلغاء هذا الحجز" };

                booking.Status = BookingStatus.Cancelled;
                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم إلغاء الحجز" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> AcceptQuoteAsync(Guid clientId, AcceptQuoteDto acceptDto)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(acceptDto.BookingId);
                if (booking == null || booking.ClientId != clientId)
                    return new ResultDto { Success = false, Message = "الحجز غير موجود" };

                var quote = await _unitOfWork.Quotes.GetByIdAsync(acceptDto.QuoteId);
                if (quote == null || quote.BookingId != acceptDto.BookingId)
                    return new ResultDto { Success = false, Message = "العرض غير صحيح" };

                if (quote.Status != QuoteStatus.Sent)
                    return new ResultDto { Success = false, Message = "العرض غير متاح" };

                quote.Status = QuoteStatus.Accepted;
                _unitOfWork.Quotes.Update(quote);

                booking.Status = BookingStatus.Accepted;
                booking.CraftsmanId = quote.CraftsmanId;
                booking.PriceFinal = quote.Price;
                booking.PaymentMethod = Enum.Parse<PaymentMethod>(acceptDto.PaymentMethod);
                _unitOfWork.Bookings.Update(booking);

                var otherQuotes = await _unitOfWork.Quotes.FindAsync(q => q.BookingId == acceptDto.BookingId && q.Id != acceptDto.QuoteId);
                foreach (var otherQuote in otherQuotes)
                {
                    otherQuote.Status = QuoteStatus.Rejected;
                    _unitOfWork.Quotes.Update(otherQuote);
                }

                await _unitOfWork.CompleteAsync();

                await _notificationService.NotifyQuoteAcceptedAsync(quote.CraftsmanId, quote.Id);

                return new ResultDto { Success = true, Message = "تم قبول العرض بنجاح" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> CompleteBookingAsync(Guid bookingId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
                if (booking == null)
                    return new ResultDto { Success = false, Message = "الحجز غير موجود" };

                booking.Status = BookingStatus.Completed;
                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.CompleteAsync();

                await _notificationService.NotifyBookingCompletedAsync(booking.ClientId, bookingId);

                return new ResultDto { Success = true, Message = "تم إتمام الحجز" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<PaginatedResultDto<BookingListItemDto>>> GetBookingsPaginatedAsync(int pageNumber, int pageSize, string? status = null)
        {
            try
            {
                var (bookings, totalCount) = await _unitOfWork.Bookings.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    predicate: status != null ? b => b.Status.ToString() == status : null,
                    orderBy: b => b.CreatedAt,
                    ascending: false
                );

                var bookingDtos = _mapper.Map<IEnumerable<BookingListItemDto>>(bookings);

                var result = new PaginatedResultDto<BookingListItemDto>
                {
                    Items = bookingDtos,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return new ResultDto<PaginatedResultDto<BookingListItemDto>> { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new ResultDto<PaginatedResultDto<BookingListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }
    }
}
