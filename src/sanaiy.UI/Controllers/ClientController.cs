using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sanaiy.BLL.DTOs.Client;   // ⬅️ مهم!
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
using sanaiy.DAL.Context;
using sanaiy.UI.ViewModels;
using System.Security.Claims;

namespace sanaiy.UI.Controllers
{
    [Authorize(Roles = "client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IBookingService _bookingService;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;


        public ClientController(
        IClientService clientService,
        IBookingService bookingService,
        IFileService fileService,
        ApplicationDbContext context)
        {
            _clientService = clientService;
            _bookingService = bookingService;
            _fileService = fileService;
            _context = context;
        }

        private Guid GetLoggedInUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (claim == null)
                throw new Exception("Not logged in");

            return Guid.Parse(claim.Value);
        }

        // ================================
        // PROFILE
        // ================================
        public async Task<IActionResult> Profile()
        {
            Guid clientId = GetLoggedInUserId();

            var profile = await _clientService.GetProfileAsync(clientId);
            var bookings = await _bookingService.GetClientBookingsAsync(clientId);

            if (!profile.Success)
                return Content(profile.Message);

            var dto = profile.Data;

            var vm = new ClientProfileViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Location = dto.Location,
                ProfileImageUrl =
                    string.IsNullOrEmpty(dto.ProfileImageUrl)
                        ? "/images/client-default.png"
                        : (dto.ProfileImageUrl.StartsWith("/") ? dto.ProfileImageUrl : "/" + dto.ProfileImageUrl),

                TotalBookings = dto.TotalBookings,
                CompletedBookings = dto.CompletedBookings,

                BookedServices = new List<BookingViewModel>()
            };

            // ---------------------------
            //  MAP BOOKINGS
            // ---------------------------
            foreach (var b in bookings.Data)
            {
                vm.BookedServices.Add(new BookingViewModel
                {
                    Id = b.Id,
                    CraftsmanName = b.CraftsmanName,
                    CraftsmanImageUrl = b.CraftsmanProfileImageUrl,
                    ClientName = b.ClientName,
                    ServiceName = b.ServiceName,
                    ShortDescription = null,

                    //Status = b.Status,
                    StatusText = GetStatusText(b.Status),
                    StatusBadgeCss = GetStatusCss(b.Status),

                    Date = b.PreferredDate,
                    Location = b.ClientAddressCity,

                    // These fields DO NOT exist in this version
                    Notes = null,
                    ShowPaymentButton = false
                });
            }

            return View(vm);
        }

        // ================================
        // EDIT PROFILE (GET)
        // ================================
        public async Task<IActionResult> EditProfile()
        {
            Guid clientId = GetLoggedInUserId();

            var result = await _clientService.GetProfileAsync(clientId);
            if (!result.Success)
                return Content(result.Message);

            var dto = new UpdateClientProfileDto
            {
                FName = result.Data.FName,
                LName = result.Data.LName,
                Phone = result.Data.Phone,
                City = result.Data.Location,
                ProfileImagePath = result.Data.ProfileImageUrl
            };

            return View(dto);
        }

        // ================================
        // EDIT PROFILE (POST)
        // ================================
        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateClientProfileDto dto)
        {
            Guid clientId = GetLoggedInUserId();

            // ✔️ 1) التحقق من صحة البيانات
            if (!ModelState.IsValid)
            {
                // رجع نفس الصفحة مع الأخطاء
                return View(dto);
            }

            // ✔️ 2) رفع صورة جديدة إن وجدت
            if (dto.ProfileImage != null)
            {
                using var ms = new MemoryStream();
                await dto.ProfileImage.CopyToAsync(ms);

                var upload = await _fileService.UploadImageAsync(
                    ms.ToArray(),
                    dto.ProfileImage.FileName,
                    "clients/profile");

                if (upload.Success)
                    dto.ProfileImagePath = upload.FilePath;
            }

            dto.City = Request.Form["City"];

            // ✔️ 3) تحديث البيانات
            var update = await _clientService.UpdateProfileAsync(clientId, dto);

            if (!update.Success)
                return Content(update.Message);

            // ✔️ 4) تحديث الـ Claims
            var newProfile = await _clientService.GetProfileAsync(clientId);

            var existingClaims = User.Claims.ToList();
            existingClaims.RemoveAll(c => c.Type == "ProfileImageUrl");

            existingClaims.Add(new Claim(
                "ProfileImageUrl",
                string.IsNullOrEmpty(newProfile.Data.ProfileImageUrl)
                    ? "/images/client-default.png"
                    : (newProfile.Data.ProfileImageUrl.StartsWith("/")
                        ? newProfile.Data.ProfileImageUrl
                        : "/" + newProfile.Data.ProfileImageUrl)
            ));

            var identity = new ClaimsIdentity(existingClaims, "sanaiyCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("sanaiyCookie", principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(30)
                });

            TempData["SuccessMessage"] = "Saved";

            return RedirectToAction("EditProfile");
        }
        private string GetStatusText(string status)
        {
            return status switch
            {
                "Pending" => "Pending",
                "Accepted" => "In Progress",
                "Completed" => "Completed Successfully",
                "Cancelled" => "Cancelled",
                _ => status
            };
        }

        private string GetStatusCss(string status)
        {
            return status switch
            {
                "Pending" => "bg-yellow-100 text-yellow-700",
                "Accepted" => "bg-blue-100 text-blue-700",
                "Completed" => "bg-green-100 text-green-700",
                "Cancelled" => "bg-red-100 text-red-700",
                _ => "bg-gray-200 text-gray-600"
            };
        }

        public async Task<IActionResult> BookingDetails(Guid id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);

            if (!result.Success)
                return Content("Booking not found");

            return PartialView("_BookingDetailsModal", result.Data);
        }


        public async Task<IActionResult> Home()
        {
            Guid id = Guid.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var result = await _clientService.GetProfileAsync(id);

            if (result.Success)
            {
                ViewBag.ProfileImageUrl = string.IsNullOrEmpty(result.Data.ProfileImageUrl)
                    ? "/images/client-default.png"
                    : (result.Data.ProfileImageUrl.StartsWith("/")
                        ? result.Data.ProfileImageUrl
                        : "/" + result.Data.ProfileImageUrl);
            }
            else
            {
                ViewBag.ProfileImageUrl = "/images/client-default.png";
            }

            return View();
        }

        // ================================
        // SETTINGS PAGE
        // ================================
        public IActionResult Setting()
        {
            Guid clientId = GetLoggedInUserId();
            return View();
        }
        [HttpGet]
        public IActionResult Violation()
        {
            return View(new ViolationViewModel { IncidentDate = DateTime.Today });
        }

        [HttpPost]
        public IActionResult Violation(ViolationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var violation = new Violation
                {
                    Id = Guid.NewGuid(),
                    Title = model.ViolationType ?? string.Empty,
                    Description = model.Description,
                    Status = ViolationStatus.Submitted,
                    ReporterUserId = GetLoggedInUserId(),
                    TargetBookingId = model.TargetBookingId,
                    TargetUserId = model.TargetUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Violations.Add(violation);
                _context.SaveChanges();

                TempData["Success"] = "Violation reported successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving violation: {ex.Message}");
                return View(model);
            }
        }


        }
}
