using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sanaiy.BLL.Interfaces;
using sanaiy.UI.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using sanaiy.BLL.Entities;
using System.Text.Json;
using System.IO;
using sanaiy.BLL.DTOs.Service;
using Microsoft.EntityFrameworkCore;

namespace sanaiy.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientService _clientService;

        public AdminController(
            ICategoryService categoryService,
            IUnitOfWork unitOfWork,
            IClientService clientService)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
            _clientService = clientService;
        }

        // Helper: ‰Õœœ ≈–« ﬂ«‰ «·ÿ·» AJAX Ê·« ·«
        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase
            );
        }

        // GET: /Admin/Dashboard  √Ê /Admin/Dashboard?page=...
        [HttpGet]
        public async Task<IActionResult> Dashboard(string? page = null)
        {
            // ·Ê «·ÿ·» „‘ AJAX ? —Ã¯⁄ ’›Õ… «·œ«‘»Ê—œ «·√”«”Ì… «··Ì ›ÌÂ« «·”«Ìœ»«—
            if (!IsAjaxRequest())
                return View("dashboard-admin");

            // „‰ Â‰« «·ÿ·» Ã«Ì „‰ JS (admin-dashboard.js)
            page = page?.Trim().ToLowerInvariant();

            switch (page)
            {
                case null:
                case "":
                case "dashboard":
                {
                    // build dashboard stats from database
                    var totalCraftsmen = await _unitOfWork.Craftsmen.GetQueryable().CountAsync();
                    var totalServices = await _unitOfWork.Services.GetQueryable().CountAsync();
                    var totalBookings = await _unitOfWork.Bookings.GetQueryable().CountAsync();
                    var totalClients = await _unitOfWork.Clients.GetQueryable().CountAsync();

                    var recentBookingsEntities = (await _unitOfWork.Bookings.GetWithIncludesAsync(
                        b => b.IsActive,
                        b => b.Service,
                        b => b.Client,
                        b => b.Craftsman,
                        b => b.Location
                    ))
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToList();

                    var recentBookings = recentBookingsEntities.Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        Date = b.PreferredDate,
                        PreferredTime = b.PreferredTime,
                        ClientName = b.Client?.FullName,
                        CraftsmanName = b.Craftsman?.FullName ?? b.Service?.OwnerCraftsman?.FullName,
                        ServiceName = b.Service?.ServiceName,
                        PriceFinal = b.PriceFinal,
                        StatusText = b.Status.ToString(),
                        Location = b.Location?.City ?? b.Location?.FullAddress
                    }).ToList();

                    var vm = new DashboardViewModel
                    {
                        TotalCraftsmen = totalCraftsmen,
                        TotalServices = totalServices,
                        TotalBookings = totalBookings,
                        TotalClients = totalClients,
                        RecentBookings = recentBookings
                    };

                    return PartialView("_DashboardPartial", vm);
                }

                case "clients":
                    return await RenderClientsPartial();

                case "category":
                    // ‰⁄Ìœ «” Œœ«„ √ﬂ‘‰ Category «·„ÊÃÊœ  Õ 
                    return await Category();

                case "craftmen":
                case "craftsmen":
                case "craftsmen-all":
                    // load craftsmen from database via unit of work
                    var craftsmen = await _unitOfWork.Craftsmen.GetWithIncludesAsync(predicate: null);

                    return View("Craftmen", craftsmen);

                case "services":
                    // load services with related category and owner craftsman
                    var services = await _unitOfWork.Services.GetWithIncludesAsync(
                        predicate: s => s.IsActive,
                        s => s.Category,
                        s => s.OwnerCraftsman
                    );

                    var dtos = services.Select(service => new ServiceListItemDto
                    {
                        Id = service.Id,
                        ServiceName = service.ServiceName,
                        Description = service.Description,
                        IsPriceFixed = service.IsPriceFixed,
                        IsActive = service.IsActive,
                        CategoryId = service.CategoryId,
                        CategoryName = service.Category?.Name ?? "Unknown",
                        CraftsmanId = service.OwnerCraftsmanId,
                        CraftsmanName = service.OwnerCraftsman != null ? (service.OwnerCraftsman.Fname + " " + service.OwnerCraftsman.Lname) : "-",
                        CraftsmanRating = service.OwnerCraftsman?.RatingAverage ?? 0m,
                        CraftsmanProfileImageUrl = service.OwnerCraftsman?.ProfileImageUrl
                    }).ToList();

                    // If request is AJAX (loader), return partial fragment only
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return PartialView("_ServicesPartial", dtos);

                    // Otherwise return full view for direct navigation
                    return View("Services", dtos);

                case "bookings":
                {
                    // load bookings with related entities
                    var bookings = await _unitOfWork.Bookings.GetWithIncludesAsync(
                        predicate: b => b.IsActive,
                        b => b.Service,
                        b => b.Client,
                        b => b.Craftsman,
                        b => b.Location
                    );

                    var bookingVms = bookings.Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        Date = b.PreferredDate,
                        PreferredTime = b.PreferredTime,
                        ClientName = b.Client?.FullName,
                        ClientImageUrl = b.Client?.ProfileImageUrl,
                        CraftsmanName = b.Craftsman?.FullName ?? b.Service?.OwnerCraftsman?.FullName,
                        CraftsmanImageUrl = b.Craftsman?.ProfileImageUrl ?? b.Service?.OwnerCraftsman?.ProfileImageUrl,
                        ServiceName = b.Service?.ServiceName,
                        ShortDescription = b.Service?.Description,
                        Category = b.Service?.Category?.Name,
                        StatusText = b.Status.ToString(),
                        PriceFinal = b.PriceFinal,
                        Location = b.Location?.FullAddress ?? b.Location?.City,
                        AdditionalNote = b.AdditionalNote
                    }).ToList();

                    return PartialView("_BookingsPartial", bookingVms);
                }

                default:
                    return Content("<p>Page not found.</p>", "text/html");
            }
        }

        // == Clients (··«” Œœ«„ ﬂ‹ Partial œ«Œ· «·œ«‘»Ê—œ) ==
        private async Task<IActionResult> RenderClientsPartial()
        {
            var result = await _clientService.GetAllClientsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return PartialView(
                    "Clients",
                    Enumerable.Empty<sanaiy.BLL.DTOs.Client.ClientListItemDto>()
                );
            }

            // View «”„Â« Clients.cshtml Ê  ⁄—÷ ÃÊ¯Â «·‹ pageContent
            return PartialView("Clients", result.Data);
        }

        // ·Ê «Õ Ã   › Õ /Admin/Clients „»«‘—…
        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            var result = await _clientService.GetAllClientsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(
                    Enumerable.Empty<sanaiy.BLL.DTOs.Client.ClientListItemDto>()
                );
            }

            return View(result.Data);
        }

        [HttpGet("ClientDetails/{id}")]
        public async Task<IActionResult> ClientDetails(Guid id)
        {
            var profileRes = await _clientService.GetProfileAsync(id);
            if (!profileRes.Success)
            {
                TempData["Error"] = profileRes.Message;
                return RedirectToAction("Clients");
            }

            return View(profileRes.Data);
        }

        // == Category list (Partial ··œ«‘»Ê—œ √Ê ’›Õ… „” ﬁ·…) ==
        [HttpGet]
        public async Task<IActionResult> Category()
        {
            var result = await _categoryService.GetAllCategoriesAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return PartialView("category", Enumerable.Empty<CategoryListItemViewModel>());
            }

            var vm = result.Data.Select(d => new CategoryListItemViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ImageUrl = d.ImageUrl,
                IsActive = d.IsActive
            }).ToList();

            return PartialView("category", vm);
        }

        // == Add Category (Ajax POST) ==
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest? req)
        {
            // ·Ê «·»«Ì‰œÌ‰Ã ›‘·° ‰Õ«Ê· ‰ﬁ—√ «·‹ body ÌœÊÌ
            if (req == null)
            {
                try { Request.Body.Position = 0; } catch { }

                try
                {
                    using var reader = new StreamReader(Request.Body);
                    var body = await reader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        req = JsonSerializer.Deserialize<AddCategoryRequest>(
                            body,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                }
                catch
                {
                    // ‰ Ã«Â· ÊÂ‰—Ã⁄ —”«·…  Õ 
                }
            }

            if (req == null || string.IsNullOrWhiteSpace(req.Name))
                return Json(new { success = false, message = "Name is required or request body missing" });

            try
            {
                // ‰ √ﬂœ „« ›Ì‘ «”„ „ﬂ—— (case-insensitive)
                var normalizedName = req.Name.Trim().ToLower();
                var exists = _unitOfWork.Categories
                    .GetQueryable()
                    .Any(c => c.Name.ToLower() == normalizedName);

                if (exists)
                    return Json(new { success = false, message = "Category with this name already exists" });

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = req.Name.Trim(),
                    Description = req.Description,
                    Image = req.Icon,      // „‰ AddCategoryRequest
                    IsActive = req.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _unitOfWork.Categories.Add(category);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.InnerException?.Message
                });
            }
        }
    }
}
