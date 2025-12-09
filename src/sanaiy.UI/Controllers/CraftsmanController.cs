using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sanaiy.BLL.DTOs;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Craftsman;
using sanaiy.BLL.DTOs.Service;
using sanaiy.BLL.DTOs.Wallet;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Interfaces;
using sanaiy.DAL.Context;
using sanaiy.UI.Helpers;
using sanaiy.UI.ViewModels;
using System.Net.NetworkInformation;
using System.Security.Claims;


namespace sanaiy.UI.Controllers
{
    [Authorize(Roles = "craftsman")]
    public class CraftsmanController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ICraftsmanService _craftsmanService;
        private readonly IBookingService _bookingService;
        private readonly IFileService _fileService;
        private readonly ICategoryService _categoryService;
        private readonly ApplicationDbContext _context;

        public CraftsmanController(
            ICraftsmanService craftsmanService,
            IBookingService bookingService,
            IFileService fileService,
            ICategoryService categoryService,
            IWalletService walletService,
            ApplicationDbContext context)
        {
            _craftsmanService = craftsmanService;
            _bookingService = bookingService;
            _fileService = fileService;
            _categoryService = categoryService;
            _walletService = walletService;
            _context = context;
        }

        [HttpDelete]
        [Route("Craftsman/DeleteService/{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return Json(new { success = false, message = "Service not found" });

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Service deleted successfully" });
        }

        public IActionResult Home() => View();

        private Guid GetLoggedInId() => Guid.Parse(User.FindFirst("UserId")!.Value);

        private bool IsCraftsman() => User.FindFirst("UserType")?.Value.ToLower() == "craftsman";

        private IActionResult CheckAccess() => !IsCraftsman() ? Unauthorized("Access denied.") : null!;

        // ============================================================
        // PROFILE PAGE
        // ============================================================
        public async Task<IActionResult> Profile()
        {
            var craftsmanId = GetLoggedInId();

            var profileResult = await _craftsmanService.GetProfileAsync(craftsmanId);
            if (!profileResult.Success) return Content(profileResult.Message);

            var dto = profileResult.Data;

            var servicesResult = await _craftsmanService.GetCraftsmanServicesAsync(craftsmanId);
            var services = servicesResult.Success ? servicesResult.Data : Enumerable.Empty<ServiceListItemDto>();

            var bookingsResult = await _bookingService.GetCraftsmanBookingsAsync(craftsmanId);
            var bookings = bookingsResult.Success ? bookingsResult.Data : Enumerable.Empty<sanaiy.BLL.DTOs.Booking.BookingListItemDto>();

            var reviewsResult = await _craftsmanService.GetCraftsmanReviewsAsync(craftsmanId);
            var reviews = reviewsResult.Success ? reviewsResult.Data : Enumerable.Empty<sanaiy.BLL.DTOs.Review.ReviewListItemDto>();

            var categoriesResult = await _categoryService.GetAllCategoriesAsync();
            var categories = categoriesResult.Success ? categoriesResult.Data.ToList() : new List<sanaiy.BLL.DTOs.Category.CategoryListItemDto>();
            //var css = BookingStatusCssHelper.GetStatusCss(status); // ✅

            var vm = new CraftsmanProfileViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                ProfessionalTitle = dto.Category,
                Bio = dto.Bio,
                Email = dto.Email,
                Phone = dto.Phone,
                Location = dto.City ?? "N/A",
                ProfileImageUrl = string.IsNullOrEmpty(dto.ProfileImageUrl) ? "/images/craftsman-default.png" : dto.ProfileImageUrl,
                RatingAverage = dto.RatingAverage,
                TotalReviews = dto.TotalReviews,
                TotalBookings = dto.TotalBookings,
                CompletedBookings = dto.CompletedBookings,
                //PendingBookings = dto.TotalBookings - dto.CompletedBookings,
                ServicesOffered = services.Select(s => new CraftsmanServiceViewModel
                {
                    ServiceId = s.Id,
                    Name = s.ServiceName,
                    ShortDescription = s.Description,
                    Category = s.CategoryName,
                    Status = s.IsActive ? "Active" : "Inactive"
                }).ToList(),
                BookedServices = bookings.Select(b => new BookingViewModel
                {
                    Id = b.Id,

                    // موعد الحجز
                    Date = b.PreferredDate,
                    PreferredTime = b.PreferredTime,

                    // العميل
                    ClientName = b.ClientName,
                    ClientImageUrl = b.ClientImageUrl,

                    // الحرفي
                    CraftsmanName = b.CraftsmanName,
                    CraftsmanProfileImage = b.CraftsmanProfileImageUrl,

                    // الخدمة
                    ServiceName = b.ServiceName,

                    // الحالة (مفيش StatusText أو Badge)
                    StatusText = b.Status,
                    //StatusBadgeCss = GetStatusCss(b.Status),
                    // هكتبّهولك تحت

                    // السعر
                    PriceFinal = b.PriceFinal,
                    Category = b.Category,
                    // الموقع
                    Location = b.ClientAddressCity
                })
.ToList()
,
                Reviews = reviews.Select(r => new ReviewViewModel
                {
                    ClientName = r.ClientName,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                Categories = categories
            };

            vm.Availability = dto.CraftsmanAvailability
                .Select(a => new AvailabilityViewModel
                {
                    Day = a.Day,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();

            return View(vm);
        }

        // ============================================================
        // ADD NEW SERVICE
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] CreateServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Incorrect data" });

            dto.CraftsmanId = GetLoggedInId();

            var result = await _craftsmanService.AddNewServiceAsync(dto);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            // نرجع الكائن المضاف مباشرة
            var service = result.Data;

            return Ok(new
            {
                success = true,
                message = "Service added successfully",
                newService = new
                {
                    Id = service.Id,
                    Name = service.ServiceName,
                    ShortDescription = service.Description,
                    Category = service.CategoryName,
                    Status = service.IsActive ? "Active" : "Inactive"

                }
            });
        }

        // ============================================================
        // EDIT PROFILE (GET)
        // ============================================================
        public async Task<IActionResult> EditProfile()
        {
            var craftsmanId = GetLoggedInId();
            var result = await _craftsmanService.GetProfileAsync(craftsmanId);

            if (!result.Success) return Content(result.Message);

            var dto = result.Data;

            var vm = new UpdateCraftsmanProfileDto
            {
                FName = dto.FName,
                LName = dto.LName,
                Phone = dto.Phone,
                Bio = dto.Bio,
                City = dto.City,
                ProfileImagePath = dto.ProfileImageUrl,
                Availability = dto.CraftsmanAvailability
            };

            return View(vm);
        }

        // ============================================================
        // EDIT PROFILE (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateCraftsmanProfileDto dto)
        {
            var craftsmanId = GetLoggedInId();

            if (dto.ProfileImage != null)
            {
                using var ms = new MemoryStream();
                await dto.ProfileImage.CopyToAsync(ms);

                var upload = await _fileService.UploadImageAsync(
                    ms.ToArray(),
                    dto.ProfileImage.FileName,
                    "craftsmen/profile"
                );

                if (upload.Success) dto.ProfileImagePath = upload.FilePath;
            }

            if (!string.IsNullOrEmpty(dto.AvailabilityJson))
            {
                dto.Availability = System.Text.Json.JsonSerializer.Deserialize<List<CraftsmanAvailabilityDto>>(dto.AvailabilityJson);
            }

            var result = await _craftsmanService.UpdateProfileAsync(craftsmanId, dto);
            if (!result.Success) return Content(result.Message);

            // تحديث Claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var oldImage = claimsIdentity.FindFirst("ProfileImageUrl");
            if (oldImage != null) claimsIdentity.RemoveClaim(oldImage);
            claimsIdentity.AddClaim(new Claim("ProfileImageUrl", string.IsNullOrEmpty(dto.ProfileImagePath) ? "/images/craftsman-default.png" : "/" + dto.ProfileImagePath));

            var oldCity = claimsIdentity.FindFirst("City");
            if (oldCity != null) claimsIdentity.RemoveClaim(oldCity);
            claimsIdentity.AddClaim(new Claim("City", dto.City ?? ""));

            var identity = new ClaimsIdentity(claimsIdentity.Claims, "sanaiyCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync("sanaiyCookie");
            await HttpContext.SignInAsync("sanaiyCookie", principal);

            return RedirectToAction("Profile");
        }

        // ============================================================
        // BOOKINGS PAGE
        // ============================================================
        public async Task<IActionResult> Bookings()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var craftsmanId = GetLoggedInId();
            var result = await _bookingService.GetCraftsmanBookingsAsync(craftsmanId);

            if (!result.Success) return Content(result.Message);

            return View(result.Data);
        }



        // ============================================================
        // APPLICATION STATUS
        // ============================================================
        public async Task<IActionResult> ApplicationStatus()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var craftsmanId = GetLoggedInId();
            var result = await _craftsmanService.GetProfileAsync(craftsmanId);

            if (!result.Success) return Content(result.Message);

            return View(result.Data);
        }

        // ============================================================
        // WALLET PAGE
        // ============================================================
        public async Task<IActionResult> Wallet()
        {
            var craftsmanId = GetLoggedInId();

            // Get wallet data
            var walletResult = await _walletService.GetWalletAsync(craftsmanId);
            if (!walletResult.Success)
            {
                TempData["Error"] = walletResult.Message;
                return View(new WalletViewModel());
            }

            // Get transactions
            var transactionsResult = await _walletService.GetTransactionsAsync(craftsmanId);

            var vm = new WalletViewModel
            {
                Wallet = walletResult.Data,
                Transactions = transactionsResult.Success ? transactionsResult.Data : new List<WalletTransactionDto>(),
                PayoutRequest = new RequestPayoutDto()
            };

            return View(vm);
        }


        // ============================================================
        // REQUEST PAYOUT (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> RequestPayout(RequestPayoutDto dto)
        {
            var craftsmanId = GetLoggedInId();

            var result = await _walletService.RequestPayoutAsync(craftsmanId, dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Wallet");
            }

            TempData["Success"] = "Withdrawal request completed successfully.";
            return RedirectToAction("Wallet");
        }

    }
}
