using AutoMapper;
using sanaiy.BLL.DTOs.Address;
using sanaiy.BLL.DTOs.Auth;
using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Category;
using sanaiy.BLL.DTOs.Client;
using sanaiy.BLL.DTOs.Craftsman;
using sanaiy.BLL.DTOs.Payment;
using sanaiy.BLL.DTOs.Quote;
using sanaiy.BLL.DTOs.Review;
using sanaiy.BLL.DTOs.Service;
using sanaiy.BLL.DTOs.Violation;
using sanaiy.BLL.DTOs.Wallet;
using sanaiy.BLL.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sanaiy.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ============================================
            // Client Mappings
            // ============================================

            CreateMap<RegisterClientDto, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.UserStatus.PendingEmailConfirmation));

            CreateMap<Client, ClientProfileDto>()
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.CompletedBookings, opt => opt.MapFrom(src =>
                    src.Bookings.Count(b => b.Status == Enums.BookingStatus.Completed)));

            CreateMap<UpdateClientProfileDto, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImagePath));

            CreateMap<Client, ClientListItemDto>()
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count));

            // ============================================
            // Craftsman Mappings
            // ============================================

            CreateMap<RegisterCraftsmanDto, Craftsman>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.CraftsmanApplicationStatus.Applied))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.RatingAverage, opt => opt.MapFrom(src => 0m))
                .ForMember(dest => dest.IDCardImage, opt => opt.MapFrom(src => src.IDCardImagePath))
                .ForMember(dest => dest.DrugTestFile, opt => opt.MapFrom(src => src.DrugTestFilePath))
                .ForMember(dest => dest.CriminalRecordFile, opt => opt.MapFrom(src => src.CriminalRecordFilePath))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl));


            CreateMap<Craftsman, CraftsmanProfileDto>()
                .ForMember(dest => dest.City,
               opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.ReviewsReceived.Count))
                .ForMember(dest => dest.WalletBalance, opt => opt.MapFrom(src => src.Wallet != null ? src.Wallet.Balance : 0))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.CompletedBookings, opt => opt.MapFrom(src =>
                    src.Bookings.Count(b => b.Status == Enums.BookingStatus.Completed)));

            CreateMap<UpdateCraftsmanProfileDto, Craftsman>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImagePath));

            CreateMap<Craftsman, CraftsmanListItemDto>()
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.ReviewsReceived.Count));

            CreateMap<Craftsman, CraftsmanApplicationDto>();

            // ============================================
            // Service Mappings
            // ============================================

            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<Service, ServiceListItemDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.OwnerCraftsman.FullName))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.OwnerCraftsman.RatingAverage));

            CreateMap<Service, ServiceDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.OwnerCraftsman.FullName))
                .ForMember(dest => dest.CraftsmanProfileImage, opt => opt.MapFrom(src => src.OwnerCraftsman.ProfileImageUrl))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.OwnerCraftsman.RatingAverage))
                .ForMember(dest => dest.CraftsmanYearsOfExperience, opt => opt.MapFrom(src => src.OwnerCraftsman.YearsOfExperience ?? 0))
                .ForMember(dest => dest.CraftsmanBio, opt => opt.MapFrom(src => src.OwnerCraftsman.Bio))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.CompletedBookings, opt => opt.MapFrom(src =>
                    src.Bookings.Count(b => b.Status == Enums.BookingStatus.Completed)));

            CreateMap<UpdateServiceDto, Service>();

            // ============================================
            // Category Mappings
            // ============================================

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<Category, CategoryListItemDto>()
                .ForMember(dest => dest.ServicesCount, opt => opt.MapFrom(src => src.Services.Count))
                .ForMember(dest => dest.CraftsmenCount, opt => opt.MapFrom(src =>
                    src.Services.Select(s => s.OwnerCraftsmanId).Distinct().Count()));

            CreateMap<Category, CategoryDetailsDto>()
                .ForMember(dest => dest.ServicesCount, opt => opt.MapFrom(src => src.Services.Count))
                .ForMember(dest => dest.CraftsmenCount, opt => opt.MapFrom(src =>
                    src.Services.Select(s => s.OwnerCraftsmanId).Distinct().Count()))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src =>
                    src.Services.SelectMany(s => s.Bookings).Count()));

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImagePath));

            // ============================================
            // Booking Mappings
            // ============================================

            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.BookingStatus.Pending))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.AddressId));

            CreateMap<Booking, BookingListItemDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName))
                .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Client.Phone))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.FullName : null))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.RatingAverage : (decimal?)null))
                .ForMember(dest => dest.QuotesCount, opt => opt.MapFrom(src => src.Quotes.Count))
                .ForMember(dest => dest.HasReview, opt => opt.MapFrom(src => src.Review != null));

            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => src.Service.Description))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName))
                .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Client.Email))
                .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Client.Phone))
                .ForMember(dest => dest.ClientProfileImage, opt => opt.MapFrom(src => src.Client.ProfileImageUrl))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.FullName : null))
                .ForMember(dest => dest.CraftsmanPhone, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.Phone : null))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.RatingAverage : (decimal?)null))
                .ForMember(dest => dest.CraftsmanProfileImage, opt => opt.MapFrom(src => src.Craftsman != null ? src.Craftsman.ProfileImageUrl : null))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.Location != null ? src.Location.FullAddress : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location != null ? src.Location.City : null))
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.LocationId))
                .ForMember(dest => dest.QuotesCount, opt => opt.MapFrom(src => src.Quotes.Count))
                .ForMember(dest => dest.HasAcceptedQuote, opt => opt.MapFrom(src => src.Quotes.Any(q => q.Status == Enums.QuoteStatus.Accepted)))
                .ForMember(dest => dest.HasPayment, opt => opt.MapFrom(src => src.PaymentTransaction != null))
                .ForMember(dest => dest.HasReview, opt => opt.MapFrom(src => src.Review != null));
                

            // ============================================
            // Quote Mappings
            // ============================================

            CreateMap<CreateQuoteDto, Quote>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.QuoteStatus.Sent))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom((src, dest, _, context) =>
                    DateTime.UtcNow.AddHours(src.ExpiryHours ?? 48)));

            CreateMap<Quote, QuoteListItemDto>()
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src =>
                    src.ExpiresAt.HasValue && src.ExpiresAt.Value < DateTime.UtcNow))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman.FullName))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.Craftsman.RatingAverage))
                .ForMember(dest => dest.CraftsmanYearsOfExperience, opt => opt.MapFrom(src => src.Craftsman.YearsOfExperience ?? 0))
                .ForMember(dest => dest.CraftsmanProfileImage, opt => opt.MapFrom(src => src.Craftsman.ProfileImageUrl))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.Service.ServiceName));

            CreateMap<Quote, QuoteDetailsDto>()
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src =>
                    src.ExpiresAt.HasValue && src.ExpiresAt.Value < DateTime.UtcNow))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.Service.ServiceName))
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.Booking.Status.ToString()))
                .ForMember(dest => dest.PreferredDate, opt => opt.MapFrom(src => src.Booking.PreferredDate))
                .ForMember(dest => dest.PreferredTime, opt => opt.MapFrom(src => src.Booking.PreferredTime))
                .ForMember(dest => dest.AdditionalNote, opt => opt.MapFrom(src => src.Booking.AdditionalNote))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman.FullName))
                .ForMember(dest => dest.CraftsmanPhone, opt => opt.MapFrom(src => src.Craftsman.Phone))
                .ForMember(dest => dest.CraftsmanRating, opt => opt.MapFrom(src => src.Craftsman.RatingAverage))
                .ForMember(dest => dest.CraftsmanYearsOfExperience, opt => opt.MapFrom(src => src.Craftsman.YearsOfExperience))
                .ForMember(dest => dest.CraftsmanProfileImage, opt => opt.MapFrom(src => src.Craftsman.ProfileImageUrl))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Booking.Client.FullName))
                .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Booking.Client.Phone))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.Booking.Location != null ? src.Booking.Location.FullAddress : null));
                
            // ============================================
            // Review Mappings
            // ============================================

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Review, ReviewListItemDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName))
                .ForMember(dest => dest.ClientProfileImage, opt => opt.MapFrom(src => src.Client.ProfileImageUrl))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.Service.ServiceName));

            CreateMap<Review, ReviewDetailsDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName))
                .ForMember(dest => dest.ClientProfileImage, opt => opt.MapFrom(src => src.Client.ProfileImageUrl))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman.FullName))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.Service.ServiceName))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Booking.PreferredDate));

            // ============================================
            // Address Mappings
            // ============================================

            CreateMap<CreateAddressDto, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Address, AddressListItemDto>();

            CreateMap<UpdateAddressDto, Address>();

            // ============================================
            // Payment Mappings
            // ============================================

            CreateMap<CreatePaymentDto, PaymentTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.PaymentStatus.Pending));

            CreateMap<PaymentTransaction, PaymentDetailsDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.Service.ServiceName))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Booking.Client.FullName))
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src =>
                    src.Booking.Craftsman != null ? src.Booking.Craftsman.FullName : null));

            // ============================================
            // Wallet Mappings
            // ============================================

            CreateMap<Wallet, WalletDetailsDto>()
                .ForMember(dest => dest.CraftsmanName, opt => opt.MapFrom(src => src.Craftsman.FullName))
                .ForMember(dest => dest.TotalTransactions, opt => opt.MapFrom(src => src.Transactions.Count))
                .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src =>
                    src.Transactions.Where(t => t.Direction == Enums.TransactionDirection.In).Sum(t => t.Amount)))
                .ForMember(dest => dest.TotalCommissions, opt => opt.MapFrom(src =>
                    src.Transactions.Where(t => t.Type == Enums.WalletTransactionType.Commission).Sum(t => t.Amount)))
                .ForMember(dest => dest.TotalPayouts, opt => opt.MapFrom(src =>
                    src.Transactions.Where(t => t.Type == Enums.WalletTransactionType.Payout).Sum(t => t.Amount)));

            CreateMap<WalletTransaction, WalletTransactionDto>()
                .ForMember(dest => dest.BookingReference, opt => opt.MapFrom(src =>
                    src.RelatedBooking != null ? $"Booking #{src.RelatedBookingId}" : null));

            // ============================================
            // Violation Mappings
            // ============================================

            CreateMap<CreateViolationDto, Violation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.ViolationStatus.Submitted));

            CreateMap<Violation, ViolationListItemDto>();
            CreateMap<Violation, ViolationDetailsDto>();
        }
    }
}