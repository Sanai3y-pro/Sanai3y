using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using sanaiy.BLL.Interfaces;
using sanaiy.BLL.DTOs.Booking;
using sanaiy.BLL.DTOs.Address;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace sanaiy.UI.Controllers
{
    // All actions require authentication
    [Authorize]
    [Route("booking")]
    public class BookingController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly IBookingService _bookingService;
        private readonly IAddressService _addressService;

        // Inject required services via constructor
        public BookingController(
            IServiceService serviceService,
            IBookingService bookingService,
            IAddressService addressService)
        {
            _serviceService = serviceService;
            _bookingService = bookingService;
            _addressService = addressService;
        }

        // Step 1: Display location selection page
        [HttpGet("location")]
        public async Task<IActionResult> Location(Guid serviceId)
        {
            // Validate service exists
            var serviceResult = await _serviceService.GetServiceByIdAsync(serviceId);
            if (!serviceResult.Success || serviceResult.Data == null)
            {
                TempData["ErrorMessage"] = "Service not available.";
                return RedirectToAction("Index", "Category");
            }

            // Check if user is logged in
            var clientId = GetCurrentClientId();
            if (clientId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Please login first.";
                return RedirectToAction("Login", "UserAuth", new
                {
                    returnUrl = Url.Action("Location", "Booking", new { serviceId })
                });
            }

            // Load user's saved addresses
            var addressesResult = await _addressService.GetUserAddressesAsync(clientId, "Client");

            ViewBag.Service = serviceResult.Data;
            ViewBag.ServiceId = serviceId;
            ViewBag.Addresses = addressesResult.Success && addressesResult.Data != null
                ? addressesResult.Data.ToList()
                : new List<AddressListItemDto>();

            // Egyptian governorates list for address creation
            var egyptGovernorates = new List<string>
            {
                "Cairo","Giza","Alexandria","Dakahlia","Gharbia","Beheira","Fayoum",
                "Minya","Beni Suef","Asyut","Sohag","Qena","Luxor","Aswan","Damietta",
                "Ismailia","Suez","Port Said","North Sinai","South Sinai","Red Sea",
                "New Valley","Matrouh","Kafr El Sheikh","Monufia","Qalyubia"
            };
            ViewBag.Governorates = egyptGovernorates;

            return View();
        }

        // Step 1: Save selected address and proceed to schedule
        [HttpPost("location")]
        public IActionResult SaveLocation(Guid serviceId, Guid addressId)
        {
            // Validate address selection
            if (addressId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Please select an address.";
                return RedirectToAction(nameof(Location), new { serviceId });
            }

            // Store booking data in session for multi-step process
            HttpContext.Session.SetString("BookingServiceId", serviceId.ToString());
            HttpContext.Session.SetString("BookingAddressId", addressId.ToString());

            return RedirectToAction(nameof(Schedule));
        }

        // Step 2: Display schedule selection page
        [HttpGet("schedule")]
        public async Task<IActionResult> Schedule()
        {
            // Ensure previous steps are completed
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BookingServiceId")) ||
                string.IsNullOrEmpty(HttpContext.Session.GetString("BookingAddressId")))
            {
                return RedirectToAction(nameof(Location));
            }

            // Load service details
            var serviceId = Guid.Parse(HttpContext.Session.GetString("BookingServiceId"));
            var serviceResult = await _serviceService.GetServiceByIdAsync(serviceId);

            if (!serviceResult.Success || serviceResult.Data == null)
            {
                TempData["ErrorMessage"] = "Service not available.";
                return RedirectToAction("Index", "Category");
            }

            ViewBag.Service = serviceResult.Data;
            ViewBag.ServiceId = serviceId;
            ViewBag.AddressId = HttpContext.Session.GetString("BookingAddressId");
            ViewBag.AdditionalNote = HttpContext.Session.GetString("BookingAdditionalNote");

            return View();
        }

        // Step 2: Save selected date/time and proceed to confirmation
        [HttpPost("schedule")]
        public IActionResult SaveSchedule(DateTime preferredDate, string preferredTime)
        {
            // Ensure location step is completed
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BookingServiceId")))
            {
                return RedirectToAction(nameof(Location));
            }

            // Validate date is in the future
            if (preferredDate.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Please select a future date.";
                return RedirectToAction(nameof(Schedule));
            }

            // Validate time selection
            if (string.IsNullOrWhiteSpace(preferredTime))
            {
                TempData["ErrorMessage"] = "Please select a time.";
                return RedirectToAction(nameof(Schedule));
            }

            // Store schedule data in session
            HttpContext.Session.SetString("BookingPreferredDate", preferredDate.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("BookingPreferredTime", preferredTime);

            return RedirectToAction(nameof(Confirm));
        }

        // Step 3: Display booking confirmation page
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BookingServiceId")) ||
                string.IsNullOrEmpty(HttpContext.Session.GetString("BookingAddressId")) ||
                string.IsNullOrEmpty(HttpContext.Session.GetString("BookingPreferredDate")) ||
                string.IsNullOrEmpty(HttpContext.Session.GetString("BookingPreferredTime")))
            {
                return RedirectToAction(nameof(Location));
            }

            var serviceId = Guid.Parse(HttpContext.Session.GetString("BookingServiceId"));
            var serviceResult = await _serviceService.GetServiceByIdAsync(serviceId);

            if (!serviceResult.Success || serviceResult.Data == null)
            {
                TempData["ErrorMessage"] = "Service not available.";
                return RedirectToAction("Index", "Category");
            }

            // 👇 الحل هنا
            var addressId = Guid.Parse(HttpContext.Session.GetString("BookingAddressId"));
            var addressResult = await _addressService.GetAddressByIdAsync(addressId);

            if (!addressResult.Success || addressResult.Data == null)
            {
                TempData["ErrorMessage"] = "Address not found.";
                return RedirectToAction(nameof(Location));
            }

            ViewBag.Address = addressResult.Data;

            ViewBag.Service = serviceResult.Data;
            ViewBag.ServiceId = serviceId;
            ViewBag.AdditionalNote = HttpContext.Session.GetString("BookingAdditionalNote");
            ViewBag.PreferredDate = HttpContext.Session.GetString("BookingPreferredDate");
            ViewBag.PreferredTime = HttpContext.Session.GetString("BookingPreferredTime");

            return View("~/Views/Booking/Confirm.cshtml");
        }


        // Step 3: Create final booking in database
        [HttpPost("confirm")]
        public async Task<IActionResult> CreateBooking()
        {
            try
            {
                // Parse and validate all booking data from session
                if (!Guid.TryParse(HttpContext.Session.GetString("BookingServiceId"), out var serviceId) ||
                    !Guid.TryParse(HttpContext.Session.GetString("BookingAddressId"), out var addressId) ||
                    !DateTime.TryParse(HttpContext.Session.GetString("BookingPreferredDate"), out var preferredDate))
                {
                    TempData["ErrorMessage"] = "Missing booking information.";
                    return RedirectToAction("Index", "Category");
                }

                var preferredTime = HttpContext.Session.GetString("BookingPreferredTime");
                var additionalNote = HttpContext.Session.GetString("BookingAdditionalNote");

                // Verify user authentication
                var clientId = GetCurrentClientId();
                if (clientId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Please login to book a service.";
                    return RedirectToAction("Login", "UserAuth");
                }

                // Get craftsman ID from service
                var serviceResult = await _serviceService.GetServiceByIdAsync(serviceId);
                var craftsmanId = serviceResult.Data.CraftsmanId;

                // Create booking DTO
                var bookingDto = new CreateBookingDto
                {
                    ServiceId = serviceId,
                    AddressId = addressId,
                    CraftsmanId = craftsmanId,
                    AdditionalNote = additionalNote,
                    PreferredDate = preferredDate,
                    PreferredTime = preferredTime
                };

                // Save booking to database
                var result = await _bookingService.CreateBookingAsync(clientId, bookingDto);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // لو الفورم اتعمله AJAX
                    return Json(new { success = result.Success, message = result.Message });
                }
                if (result.Success)
                {
                    // Clear session data after successful booking
                    HttpContext.Session.Remove("BookingServiceId");
                    HttpContext.Session.Remove("BookingAddressId");
                    HttpContext.Session.Remove("BookingAdditionalNote");
                    HttpContext.Session.Remove("BookingPreferredDate");
                    HttpContext.Session.Remove("BookingPreferredTime");

                    // Show success message and stay on confirmation page
                    TempData["BookingSuccessMessage"] = "Your booking has been confirmed successfully!";
                    return RedirectToAction("Confirm");

                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction(nameof(Confirm));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Category");
            }
        }

        // Save multiple selected services (if using cart functionality)
        [HttpPost("services")]
        public IActionResult SaveSelectedServices([FromForm] string selectedServicesJson, [FromForm] string? additionalNote)
        {
            // Validate service selection
            if (string.IsNullOrEmpty(selectedServicesJson))
            {
                TempData["ErrorMessage"] = "Please select at least one service.";
                return RedirectToAction("Services", "Category");
            }

            // Store selected services in session
            HttpContext.Session.SetString("BookingSelectedServices", selectedServicesJson);
            HttpContext.Session.SetString("BookingAdditionalNote", additionalNote ?? string.Empty);

            // Extract first service ID for initial booking flow
            var firstServiceId = Guid.Empty;
            try
            {
                var services = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(selectedServicesJson);
                if (services != null && services.Any())
                    firstServiceId = services.First();
            }
            catch { }

            return RedirectToAction("Location", "Booking", new { serviceId = firstServiceId });
        }

        // Ajax endpoint: Add new address without page reload
        [HttpPost("add-address-ajax")]
        public async Task<IActionResult> AddAddressAjax([FromBody] CreateAddressDto createDto)
        {
            // Verify user authentication
            var clientId = GetCurrentClientId();

            if (clientId == Guid.Empty)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            // Create address in database
            var result = await _addressService.CreateAddressAsync(clientId, "Client", createDto);

            // Return JSON response for Ajax
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                addressId = result.Data
            });
        }

        // Helper: Extract current client ID from authentication claims
        private Guid GetCurrentClientId()
        {
            if (HttpContext.Session == null) return Guid.Empty;

            if (User.Identity?.IsAuthenticated == true)
            {
                var clientIdClaim = User.FindFirst("UserId");
                if (clientIdClaim != null && Guid.TryParse(clientIdClaim.Value, out var id))
                {
                    return id;
                }
            }

            return Guid.Empty;
        }
        // BookingController
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetBookingDetails(Guid id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);

            if (!result.Success || result.Data == null)
                return Json(new { success = false, message = "Booking not found" });

            var booking = result.Data;

            return Json(new
            {
                success = true,
                booking = new
                {
                    ClientName = booking.ClientName,
                    ServiceName = booking.ServiceName,
                    FullAddress = booking.FullAddress,
                    PreferredDate = booking.PreferredDate?.ToString("yyyy-MM-dd"),
                    PreferredTime = booking.PreferredTime,
                    AdditionalNote = booking.AdditionalNote ?? "-"
                }
            });

        }


        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            if (!result.Success || result.Data == null)
                return Json(new { success = false, message = "Booking not found" });

            var booking = result.Data;

            return Json(new
            {
                success = true,
                booking = new
                {
                    booking.Id,
                    ClientName = booking.ClientName,   // لازم تكون موجودة في DTO
                    ServiceName = booking.ServiceName, // لازم تكون موجودة في DTO
                    Address = booking.FullAddress,         // لازم تكون موجودة في DTO
                    //PreferredDate = booking.PreferredDate.ToString("yyyy-MM-dd"),
                    PreferredTime = booking.PreferredTime,
                    AdditionalNote = booking.AdditionalNote
                }
            });
        }

    }
}