using Microsoft.AspNetCore.Mvc;
using sanaiy.UI.ViewModels;
using sanaiy.BLL.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace sanaiy.UI.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ICraftsmanService _craftsmanService;

        public SettingController(IClientService clientService, ICraftsmanService craftsmanService)
        {
            _clientService = clientService;
            _craftsmanService = craftsmanService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userType = GetCurrentUserType();
            var viewModel = new SettingViewModel
            {
                UserType = userType,
                FullName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name,
                Email = User.FindFirst("Email")?.Value,
                ProfileImageUrl = User.FindFirst("ProfileImageUrl")?.Value ?? "/images/default-profile.png",
                Phone = User.FindFirst("Phone")?.Value
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Setting()
        {
            return RedirectToAction("Index");
        }

        private Guid GetCurrentUserId()
        {
            // الحل السريع - ابحث عن أي ID متاح
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
                return Guid.Parse(userIdClaim.Value);

            var craftsmanIdClaim = User.FindFirst("CraftsmanId");
            if (craftsmanIdClaim != null)
                return Guid.Parse(craftsmanIdClaim.Value);

            var clientIdClaim = User.FindFirst("ClientId");
            if (clientIdClaim != null)
                return Guid.Parse(clientIdClaim.Value);

            throw new Exception("User identity not found in claims.");
        }

        private string GetCurrentUserType()
        {
            return User.FindFirst("UserType")?.Value?.ToLower() ?? "client";
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all required fields correctly.";
                return RedirectToAction("Index");
            }

            var userType = GetCurrentUserType();
            var userId = GetCurrentUserId();

            try
            {
                if (userType == "client")
                {
                    var result = await _clientService.ChangePasswordAsync(userId, model.CurrentPassword!, model.NewPassword!);

                    if (result.Success)
                    {
                        TempData["Success"] = result.Message;
                        // تسجيل الخروج بعد تغيير كلمة المرور
                        await HttpContext.SignOutAsync("sanaiyCookie");
                        TempData["Info"] = "Please login again with your new password.";
                        return RedirectToAction("Login", "UserAuth");
                    }
                    else
                    {
                        TempData["Error"] = result.Message;
                    }
                }
                else if (userType == "craftsman")
                {
                    // استخدم الدالة الجديدة
                    var result = await _craftsmanService.ChangePasswordAsync(userId, model.CurrentPassword!, model.NewPassword!);

                    if (result.Success)
                    {
                        TempData["Success"] = result.Message;
                        // تسجيل الخروج بعد تغيير كلمة المرور
                        await HttpContext.SignOutAsync("sanaiyCookie");
                        TempData["Info"] = "Please login again with your new password.";
                        return RedirectToAction("Login", "UserAuth");
                    }
                    else
                    {
                        TempData["Error"] = result.Message;
                    }
                }
                else
                {
                    TempData["Error"] = "Invalid user type.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(SettingViewModel model)
        {
            if (string.IsNullOrEmpty(model.DeletePassword))
            {
                TempData["Error"] = "Please enter your password to confirm deletion.";
                return RedirectToAction("Index");
            }

            var userType = GetCurrentUserType();
            var userId = GetCurrentUserId();

            try
            {
                if (userType == "client")
                {
                    var result = await _clientService.DeleteAccountAsync(userId, model.DeletePassword!);

                    if (result.Success)
                    {
                        await HttpContext.SignOutAsync("sanaiyCookie");
                        TempData["Success"] = "Account deleted successfully.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = result.Message;
                    }
                }
                else if (userType == "craftsman")
                {
                    // يمكنك إضافة دالة DeleteAccountAsync للحرفيين لاحقاً
                    TempData["Error"] = "Account deletion for craftsmen is not available yet. Please contact support.";
                }
                else
                {
                    TempData["Error"] = "Invalid user type.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}