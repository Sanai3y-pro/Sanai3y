using AutoMapper;
using sanaiy.BLL.DTOs;
using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Craftsman;
using sanaiy.BLL.DTOs.Review;
using sanaiy.BLL.DTOs.Service;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class CraftsmanService : ICraftsmanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CraftsmanService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        // ============================
        // GET PROFILE
        // ============================
        public async Task<ResultDto<CraftsmanProfileDto>> GetProfileAsync(Guid craftsmanId)
        {
            try
            {
                var list = await _unitOfWork.Craftsmen.GetWithIncludesAsync(
                    c => c.Id == craftsmanId,
                    c => c.Bookings,
                    c => c.Address,
                    c => c.ReviewsReceived
                );

                var craftsman = list.FirstOrDefault();

                if (craftsman == null)
                    return new ResultDto<CraftsmanProfileDto>
                    {
                        Success = false,
                        Message = "Craftsman not found"
                    };

                var dto = _mapper.Map<CraftsmanProfileDto>(craftsman);

                dto.TotalBookings = craftsman.Bookings?.Count ?? 0;
                var availability = await _unitOfWork.CraftsmanAvailability
                    .FindAsync(a => a.CraftsmanId == craftsmanId);

                dto.CraftsmanAvailability = availability
                    .Select(a => new CraftsmanAvailabilityDto
                    {
                        Id = a.Id,
                        Day = a.Day,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime
                    })
                    .ToList();

                return new ResultDto<CraftsmanProfileDto>
                {
                    Success = true,
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<CraftsmanProfileDto>
                {
                    Success = false,
                    Message = "Error loading data: " + ex.Message,
                    Errors = new() { ex.Message }
                };
            }
        }

        // ============================
        // UPDATE PROFILE
        // ============================
        public async Task<ResultDto> UpdateProfileAsync(Guid craftsmanId, UpdateCraftsmanProfileDto dto)
        {
            try
            {
                var craftsman = await _unitOfWork.Craftsmen.GetByIdAsync(craftsmanId);

                if (craftsman == null)
                    return new ResultDto { Success = false, Message = "Craftsman not found" };

                craftsman.Fname = dto.FName;
                craftsman.Lname = dto.LName;
                craftsman.Phone = dto.Phone;
                craftsman.Bio = dto.Bio;

                // Update address
                var addresses = await _unitOfWork.Addresses.FindAsync(a => a.CraftsmanId == craftsmanId && a.IsDefault);
                var existingAddress = addresses.FirstOrDefault();

                if (existingAddress == null)
                {
                    _unitOfWork.Addresses.Add(new Address
                    {
                        Id = Guid.NewGuid(),
                        CraftsmanId = craftsmanId,
                        City = dto.City ?? "",
                        FullAddress = dto.City ?? "",
                        AddressType = AddressType.Work,
                        IsDefault = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existingAddress.City = dto.City ?? existingAddress.City;
                    existingAddress.FullAddress = dto.City ?? existingAddress.FullAddress;
                    existingAddress.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Addresses.Update(existingAddress);
                }

                // Update profile image
                if (!string.IsNullOrEmpty(dto.ProfileImagePath))
                {
                    if (!string.IsNullOrEmpty(craftsman.ProfileImageUrl))
                        await _fileService.DeleteFileAsync(craftsman.ProfileImageUrl);

                    craftsman.ProfileImageUrl = dto.ProfileImagePath;
                }

                _unitOfWork.Craftsmen.Update(craftsman);

                // Update availability
                if (dto.Availability != null && dto.Availability.Any())
                {
                    var old = await _unitOfWork.CraftsmanAvailability.FindAsync(a => a.CraftsmanId == craftsmanId);
                    foreach (var slot in old) _unitOfWork.CraftsmanAvailability.Remove(slot);

                    foreach (var slot in dto.Availability)
                    {
                        _unitOfWork.CraftsmanAvailability.Add(new CraftsmanAvailability
                        {
                            Id = Guid.NewGuid(),
                            CraftsmanId = craftsmanId,
                            Day = slot.Day,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "Profile updated successfully" };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Errors = new() { ex.Message }
                };
            }
        }

        // ============================
        // GET CRAFTSMAN SERVICES
        // ============================

        public async Task<ResultDto<List<BookingListItemDto>>> GetCraftsmanBookingsAsync(Guid craftsmanId)
        {
            try
            {
                // جلب الحجوزات الخاصة بهذا الحرفي من قاعدة البيانات
                var bookings = await _unitOfWork.Bookings.FindAsync(
                    b => b.CraftsmanId == craftsmanId
                );


                if (bookings == null || !bookings.Any())
                    return new ResultDto<List<BookingListItemDto>>
                    {
                        Success = true,
                        Data = new List<BookingListItemDto>(),
                        Message = "No bookings found"
                    };

                // تحويل البيانات إلى BookingListItemDto
                var bookingDtos = bookings.Select(b => new BookingListItemDto
                {
                    Id = b.Id,
                    ServiceName = b.Service?.ServiceName ?? "-",
                    Status = b.Status.ToString(),
                    PreferredDate = b.PreferredDate,
                    PreferredTime = b.PreferredTime,
                    PriceFinal = b.PriceFinal,
                    CreatedAt = b.CreatedAt,

                    ClientName = b.Client?.FullName ?? "-",
                    ClientPhone = b.Client?.Phone ?? "-",
                    ClientAddressCity = b.Location?.City ?? "-",
                    ClientImageUrl = b.Client?.ProfileImageUrl,

                    CraftsmanName = b.Craftsman?.FullName,
                    CraftsmanRating = b.Craftsman?.RatingAverage,
                    CraftsmanProfileImageUrl = b.Craftsman?.ProfileImageUrl,

                    QuotesCount = b.Quotes?.Count ?? 0,
                    HasReview = b.Quotes?.Any() ?? false
                }).ToList();

                return new ResultDto<List<BookingListItemDto>>
                {
                    Success = true,
                    Data = bookingDtos,
                    Message = "Bookings retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<List<BookingListItemDto>>
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Errors = new List<string> { ex.Message }
                };
            }
        }



        // ============================
        // ADD NEW SERVICE
        // ============================
        public async Task<ResultDto<ServiceListItemDto>> AddNewServiceAsync(CreateServiceDto dto)
        {
            try
            {
                var craftsman = await _unitOfWork.Craftsmen.GetByIdAsync(dto.CraftsmanId);
                if (craftsman == null)
                    return new ResultDto<ServiceListItemDto> { Success = false, Message = "Craftsman not found" };

                var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
                if (category == null || !category.IsActive)
                    return new ResultDto<ServiceListItemDto> { Success = false, Message = "Category not found or inactive" };

                var service = new Service
                {
                    Id = Guid.NewGuid(),
                    ServiceName = dto.ServiceName,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    OwnerCraftsmanId = dto.CraftsmanId,
                    IsPriceFixed = dto.IsPriceFixed,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _unitOfWork.Services.Add(service);
                await _unitOfWork.CompleteAsync();

                // إرجاع الكائن مباشرة بعد الإضافة
                var serviceDto = new ServiceListItemDto
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    CategoryName = category.Name,
                    IsActive = service.IsActive
                };

                return new ResultDto<ServiceListItemDto>
                {
                    Success = true,
                    Message = "Service added successfully",
                    Data = serviceDto
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<ServiceListItemDto>
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Errors = new() { ex.Message }
                };
            }
        }

        // ============================
        // GET CRAFTSMAN REVIEWS
        // ============================
        public async Task<ResultDto<IEnumerable<ReviewListItemDto>>> GetCraftsmanReviewsAsync(Guid craftsmanId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.CraftsmanId == craftsmanId);

            var dto = reviews.Select(r => new ReviewListItemDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ClientName = r.Client?.FullName,
                ClientProfileImageUrl = r.Client?.ProfileImageUrl
            });

            return ResultDto<IEnumerable<ReviewListItemDto>>.SuccessResult(dto);
        }

        // ============================
        // CHANGE PASSWORD
        // ============================
        public async Task<ResultDto> ChangePasswordAsync(Guid craftsmanId, string currentPassword, string newPassword)
        {
            try
            {
                var craftsman = await _unitOfWork.Craftsmen.GetByIdAsync(craftsmanId);

                if (craftsman == null)
                    return new ResultDto { Success = false, Message = "Craftsman not found" };

                // 1. عرض معلومات التجزئة للتصحيح
                Console.WriteLine("=== DEBUG PASSWORD INFO ===");
                Console.WriteLine($"Craftsman ID: {craftsmanId}");
                Console.WriteLine($"Stored hash: {craftsman.Password}");
                Console.WriteLine($"Stored hash length: {craftsman.Password?.Length}");
                Console.WriteLine($"Current password input: {currentPassword}");
                Console.WriteLine($"New password input: {newPassword}");

                // 2. محاولات متعددة للتحقق من كلمة المرور
                bool isPasswordValid = false;
                string detectedHashMethod = "";

                // المحاولة 1: SHA256 (كودك الحالي)
                using var sha256 = SHA256.Create();
                var sha256Bytes = Encoding.UTF8.GetBytes(currentPassword);
                var sha256Hash = sha256.ComputeHash(sha256Bytes);
                var sha256HashBase64 = Convert.ToBase64String(sha256Hash);

                Console.WriteLine($"SHA256 Base64 hash: {sha256HashBase64}");
                Console.WriteLine($"SHA256 match: {sha256HashBase64 == craftsman.Password}");

                if (sha256HashBase64 == craftsman.Password)
                {
                    isPasswordValid = true;
                    detectedHashMethod = "SHA256 (Base64)";
                }

                // المحاولة 2: SHA256 كـ HEX string
                if (!isPasswordValid)
                {
                    var sha256Hex = BitConverter.ToString(sha256Hash).Replace("-", "").ToLower();
                    Console.WriteLine($"SHA256 Hex hash: {sha256Hex}");
                    Console.WriteLine($"SHA256 Hex match: {sha256Hex == craftsman.Password}");

                    if (sha256Hex == craftsman.Password)
                    {
                        isPasswordValid = true;
                        detectedHashMethod = "SHA256 (Hex)";
                    }
                }

                // المحاولة 3: MD5
                if (!isPasswordValid)
                {
                    using var md5 = System.Security.Cryptography.MD5.Create();
                    var md5Bytes = Encoding.UTF8.GetBytes(currentPassword);
                    var md5Hash = md5.ComputeHash(md5Bytes);
                    var md5HashBase64 = Convert.ToBase64String(md5Hash);

                    Console.WriteLine($"MD5 Base64 hash: {md5HashBase64}");
                    Console.WriteLine($"MD5 match: {md5HashBase64 == craftsman.Password}");

                    if (md5HashBase64 == craftsman.Password)
                    {
                        isPasswordValid = true;
                        detectedHashMethod = "MD5 (Base64)";
                    }
                }

                // المحاولة 4: MD5 كـ HEX
                if (!isPasswordValid)
                {
                    using var md5 = System.Security.Cryptography.MD5.Create();
                    var md5Bytes = Encoding.UTF8.GetBytes(currentPassword);
                    var md5Hash = md5.ComputeHash(md5Bytes);
                    var md5Hex = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();

                    Console.WriteLine($"MD5 Hex hash: {md5Hex}");
                    Console.WriteLine($"MD5 Hex match: {md5Hex == craftsman.Password}");

                    if (md5Hex == craftsman.Password)
                    {
                        isPasswordValid = true;
                        detectedHashMethod = "MD5 (Hex)";
                    }
                }

                // المحاولة 5: BCrypt (إذا كنت تستخدمها)
                if (!isPasswordValid)
                {
                    try
                    {
                        // تأكد من تثبيت حزمة BCrypt.Net-Next
                        // Install-Package BCrypt.Net-Next
                        bool bcryptMatch = BCrypt.Net.BCrypt.Verify(currentPassword, craftsman.Password);
                        Console.WriteLine($"BCrypt match: {bcryptMatch}");

                        if (bcryptMatch)
                        {
                            isPasswordValid = true;
                            detectedHashMethod = "BCrypt";
                        }
                    }
                    catch (Exception bcryptEx)
                    {
                        Console.WriteLine($"BCrypt check failed: {bcryptEx.Message}");
                    }
                }

                // المحاولة 6: نص عادي (للتطوير فقط - خطير!)
                if (!isPasswordValid)
                {
                    Console.WriteLine($"Plain text match: {currentPassword == craftsman.Password}");
                    if (currentPassword == craftsman.Password)
                    {
                        isPasswordValid = true;
                        detectedHashMethod = "Plain Text (WARNING!)";
                        Console.WriteLine("WARNING: Password stored in plain text!");
                    }
                }

                Console.WriteLine($"Password valid: {isPasswordValid}");
                Console.WriteLine($"Detected hash method: {detectedHashMethod}");
                Console.WriteLine("=== END DEBUG ===");

                if (!isPasswordValid)
                    return new ResultDto
                    {
                        Success = false,
                        Message = $"Current password is incorrect. Detected hash method: {detectedHashMethod}"
                    };

                // 3. التحقق من أن كلمة المرور الجديدة مختلفة
                if (currentPassword == newPassword)
                    return new ResultDto { Success = false, Message = "New password must be different from current password" };

                // 4. تحديث كلمة المرور باستخدام الطريقة المكتشفة
                string newPasswordHash = "";

                switch (detectedHashMethod)
                {
                    case "SHA256 (Base64)":
                        newPasswordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword)));
                        break;
                    case "SHA256 (Hex)":
                        var newSha256Hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                        newPasswordHash = BitConverter.ToString(newSha256Hash).Replace("-", "").ToLower();
                        break;
                    case "MD5 (Base64)":
                        using (var md5 = System.Security.Cryptography.MD5.Create())
                        {
                            newPasswordHash = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(newPassword)));
                        }
                        break;
                    case "MD5 (Hex)":
                        using (var md5 = System.Security.Cryptography.MD5.Create())
                        {
                            var newMd5Hash = md5.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                            newPasswordHash = BitConverter.ToString(newMd5Hash).Replace("-", "").ToLower();
                        }
                        break;
                    case "BCrypt":
                        newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        break;
                    case "Plain Text (WARNING!)":
                        newPasswordHash = newPassword; // لا تفعل هذا في الإنتاج!
                        break;
                    default:
                        // الافتراضي: SHA256 Base64
                        newPasswordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword)));
                        break;
                }

                craftsman.Password = newPasswordHash;
                craftsman.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Craftsmen.Update(craftsman);
                await _unitOfWork.CompleteAsync();

                return new ResultDto
                {
                    Success = true,
                    Message = $"Password changed successfully. Method: {detectedHashMethod}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password change error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return new ResultDto
                {
                    Success = false,
                    Message = $"Failed to change password: {ex.Message}",
                    Errors = new() { ex.Message }
                };
            }
        }

        // ============================
        // HELPER METHODS FOR PASSWORD
        // ============================
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(inputPassword);
            var hash = sha256.ComputeHash(bytes);
            var inputHash = Convert.ToBase64String(hash);

            return inputHash == storedHash;
        }

        // ============================
        // OTHER METHODS (Placeholders)
        // ============================
        public Task<ResultDto<IEnumerable<CraftsmanListItemDto>>> GetCraftsmenByCategoryAsync(string category)
            => Task.FromResult(new ResultDto<IEnumerable<CraftsmanListItemDto>> { Success = false });

        public Task<ResultDto<IEnumerable<CraftsmanApplicationDto>>> GetPendingApplicationsAsync()
            => Task.FromResult(new ResultDto<IEnumerable<CraftsmanApplicationDto>> { Success = false });

        public Task<ResultDto> ApproveCraftsmanAsync(ApproveCraftsmanDto approveDto)
            => Task.FromResult(new ResultDto { Success = false });

        public Task<ResultDto> RejectCraftsmanAsync(Guid id, string reason)
            => Task.FromResult(new ResultDto { Success = false });

        public Task<ResultDto<PaginatedResultDto<CraftsmanListItemDto>>> SearchCraftsmenAsync(SearchFilterDto filter)
            => Task.FromResult(new ResultDto<PaginatedResultDto<CraftsmanListItemDto>> { Success = false });

        public Task<ResultDto> ChangePasswordAsync(Guid craftsmanId, ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }
        public async Task<ResultDto<List<ServiceListItemDto>>> GetCraftsmanServicesAsync(Guid craftsmanId)
        {
            var services = await _unitOfWork.Services.FindAsync(s => s.OwnerCraftsmanId == craftsmanId);

            var dto = services.Select(s => new ServiceListItemDto
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                Description = s.Description,
                CategoryName = s.Category?.Name,
                IsActive = s.IsActive
            }).ToList();

            return new ResultDto<List<ServiceListItemDto>>
            {
                Success = true,
                Data = dto
            };
        }

    }
}