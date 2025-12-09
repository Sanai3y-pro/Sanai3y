using Microsoft.AspNetCore.Mvc;
using sanaiy.BLL.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace sanaiy.UI.Controllers
{

    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IServiceService _serviceService;

        public CategoryController(ICategoryService categoryService, IServiceService serviceService)
        {
            _categoryService = categoryService;
            _serviceService = serviceService;
        }

        // GET: /Category - Show all categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAllCategoriesAsync();

            if (result.Success)
            {
                return View(result.Data);
            }

            // If error, show empty page with message
            TempData["ErrorMessage"] = "Unable to load categories. Please try again.";
            return View(new System.Collections.Generic.List<sanaiy.BLL.DTOs.Category.CategoryListItemDto>());
        }

        // GET: /Category/Services/{id} - Show services for a specific category
        [HttpGet]
        public async Task<IActionResult> Services(Guid id)
        {
            // 1. Get category details
            var categoryResult = await _categoryService.GetCategoryByIdAsync(id);

            if (!categoryResult.Success)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Get services for this category
            var servicesResult = await _serviceService.GetServicesByCategoryAsync(id);

            // 3. Pass data to view
            ViewBag.Category = categoryResult.Data;

            if (servicesResult.Success)
            {
                return View(servicesResult.Data);
            }

            // If no services, show empty list
            TempData["InfoMessage"] = "No services available in this category yet.";
            return View(new System.Collections.Generic.List<sanaiy.BLL.DTOs.Service.ServiceListItemDto>());
        }
    }
}