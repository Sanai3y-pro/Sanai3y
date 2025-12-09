using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using sanaiy.BLL.DTOs.Auth;
using sanaiy.BLL.Interfaces;
using sanaiy.UI.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using sanaiy.BLL.Options;

namespace sanaiy.UI.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFileService _fileService;
        private readonly AdminAccountOptions _adminOptions;

        private Guid GetLoggedInUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
                throw new Exception("User is not logged in.");

            return Guid.Parse(userIdClaim.Value);
        }

        public UserAuthController(IAuthService authService, IFileService fileService, IOptions<AdminAccountOptions> adminOptions)
        {
            _authService = authService;
            _fileService = fileService;
            _adminOptions = adminOptions?.Value ?? new AdminAccountOptions();
        }

        // ============================
        // LOGIN (GET)
        // ============================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        // ============================
        // LOGIN (POST) >> Login Client OR Craftsman
        // ============================
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // ========== Admin Login (from appsettings) ==========
            try
            {
                if (!string.IsNullOrEmpty(dto.Email) && !string.IsNullOrEmpty(dto.Password))
                {
                    if (dto.Email.Trim().Equals(_adminOptions.Email?.Trim(), StringComparison.OrdinalIgnoreCase)
                        && dto.Password == _adminOptions.Password)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "Administrator"),
                            new Claim(ClaimTypes.Email, _adminOptions.Email ?? string.Empty),
                            new Claim("UserType", "admin"),
                            new Claim(ClaimTypes.Role, _adminOptions.Role ?? "Admin")
                        };

                        var identity = new ClaimsIdentity(claims, "sanaiyCookie");
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync("sanaiyCookie", principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = dto.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddDays(30)
                            });

                        return RedirectToAction("Dashboard", "Admin");
                    }
                }
            }
            catch
            {
                // ignore and continue to try other login methods
            }

            // 1) Try Client Login
            var clientLogin = await _authService.LoginClientAsync(dto);

            if (clientLogin.Success)
            {
                var claims = new List<Claim>
        {
            new Claim("UserId", clientLogin.Data.UserId.ToString()),
            new Claim(ClaimTypes.Name, clientLogin.Data.FullName),
            new Claim(ClaimTypes.Email, clientLogin.Data.Email),

            // Role & UserType
            new Claim("UserType", "client"),
            new Claim(ClaimTypes.Role, "client"),

            new Claim("ProfileImageUrl",
                clientLogin.Data.ProfileImageUrl ?? "/images/client-default.png")
        };

                var identity = new ClaimsIdentity(claims, "sanaiyCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("sanaiyCookie", principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = dto.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    });

                return RedirectToAction("Home", "Client");
            }

            // 2) Try Craftsman Login
            var craftsmanLogin = await _authService.LoginCraftsmanAsync(dto);

            if (craftsmanLogin.Success)
            {
                var claims = new List<Claim>
        {
            new Claim("UserId", craftsmanLogin.Data.UserId.ToString()),
            new Claim(ClaimTypes.Name, craftsmanLogin.Data.FullName),
            new Claim(ClaimTypes.Email, craftsmanLogin.Data.Email),

            // Role & UserType
            new Claim("UserType", "craftsman"),
            new Claim(ClaimTypes.Role, "craftsman"),

            new Claim("ProfileImageUrl",
                craftsmanLogin.Data.ProfileImageUrl ?? "/images/craftsman-default.png")
        };

                var identity = new ClaimsIdentity(claims, "sanaiyCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("sanaiyCookie", principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = dto.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    });

                return RedirectToAction("Home", "Craftsman");
            }


            // 3) Both failed → Wrong email/password
            ModelState.AddModelError("", "Invalid email or password");
            return View(dto);
        }


        // ============================
        // REGISTER STEP 1 (GET)
        // ============================
        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View(new RegisterStep1ViewModel());
        }

        // ============================
        // REGISTER STEP 1 (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> RegisterStep1(RegisterStep1ViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            Console.WriteLine($"IsDefaultAddress: {vm.IsDefaultAddress}");

            if (vm.AddressType == "other")
            {
                vm.AddressType = vm.OtherAddressType;
            }

            // ========== PROFILE IMAGE UPLOAD (for both client & craftsman) ==========
            if (vm.ProfileImage != null)
            {
                using var ms = new MemoryStream();
                await vm.ProfileImage.CopyToAsync(ms);

                string folder = vm.RegisterAs == "client"
    ? "users/clients"
    : "users/craftsmen";

                var upload = await _fileService.UploadImageAsync(
                    ms.ToArray(),
                    vm.ProfileImage.FileName,
                    folder);

                if (!upload.Success)
                {
                    ModelState.AddModelError("", upload.Message);
                    return View(vm);
                }

                vm.ProfileImagePath = upload.FilePath;
            }

            if (vm.RegisterAs == "client")
            {
                var dto = new RegisterClientDto
                {
                    FName = vm.FName,
                    LName = vm.LName,
                    Email = vm.Email,
                    Phone = vm.Phone,
                    Password = vm.Password,
                    ConfirmPassword = vm.Password,
                    City = vm.City,
                    FullAddress = vm.FullAddress,
                    IsDefaultAddress = vm.IsDefaultAddress,
                    ProfileImageUrl = vm.ProfileImagePath   // ✔ Already uploaded above
                };

                var result = await _authService.RegisterClientAsync(dto);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(vm);
                }

                return RedirectToAction("Login");
            }

            // Save Step1 data for craftsman
            var tempData = new RegisterStep1TempDto
            {
                FName = vm.FName,
                LName = vm.LName,
                Phone = vm.Phone,
                Email = vm.Email,
                Password = vm.Password,
                FullAddress = vm.FullAddress,
                City = vm.City,
                AddressType = vm.AddressType,
                IsDefaultAddress = vm.IsDefaultAddress,
                RegisterAs = vm.RegisterAs,
                ProfileImagePath = vm.ProfileImagePath
            };

            TempData["Step1"] = JsonSerializer.Serialize(tempData);
            TempData.Keep("Step1");
            return RedirectToAction("RegisterStep2");
        }


        // ============================
        // REGISTER STEP 2 (GET)
        // ============================
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            if (TempData["Step1"] == null)
                return RedirectToAction("RegisterStep1");

            TempData.Keep("Step1");
            return View(new RegisterStep2ViewModel());
        }


        // ============================
        // REGISTER STEP 2 (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (!TempData.ContainsKey("Step1"))
                return RedirectToAction("RegisterStep1");

            var step1Json = TempData["Step1"]!.ToString();
            TempData.Keep("Step1");

            var step1 = JsonSerializer.Deserialize<RegisterStep1TempDto>(step1Json!)!;

            var dto = new RegisterCraftsmanDto
            {
                FName = step1.FName,
                LName = step1.LName,
                Email = step1.Email,
                Phone = step1.Phone,
                Password = step1.Password,
                ConfirmPassword = step1.Password,
                NationalID = vm.NationalID,
                YearsOfExperience = vm.YearsOfExperience!.Value,
                Category = vm.Category,
                Bio = vm.Bio,
                ProfileImageUrl = step1.ProfileImagePath ?? "/images/craftsman-default.png",
                City = step1.City,
                FullAddress = step1.FullAddress
            };

            // Upload ID
            if (vm.IDCardImage != null)
            {
                using var ms = new MemoryStream();
                await vm.IDCardImage.CopyToAsync(ms);
                var upload = await _fileService.UploadImageAsync(ms.ToArray(),
                                                                vm.IDCardImage.FileName,
                                                                "craftsmen/idcards");

                if (!upload.Success)
                {
                    ModelState.AddModelError("", upload.Message);
                    return View(vm);
                }

                dto.IDCardImagePath = upload.FilePath;
            }

            // Upload Drug Test
            if (vm.DrugTestFile != null)
            {
                using var ms = new MemoryStream();
                await vm.DrugTestFile.CopyToAsync(ms);
                var upload = await _fileService.UploadDocumentAsync(ms.ToArray(),
                                                                    vm.DrugTestFile.FileName,
                                                                    "craftsmen/drugtests");

                if (!upload.Success)
                {
                    ModelState.AddModelError("", upload.Message);
                    return View(vm);
                }

                dto.DrugTestFilePath = upload.FilePath;
            }

            // Upload Criminal Record
            if (vm.CriminalRecordFile != null)
            {
                using var ms = new MemoryStream();
                await vm.CriminalRecordFile.CopyToAsync(ms);
                var upload = await _fileService.UploadDocumentAsync(ms.ToArray(),
                                                                    vm.CriminalRecordFile.FileName,
                                                                    "craftsmen/records");

                if (!upload.Success)
                {
                    ModelState.AddModelError("", upload.Message);
                    return View(vm);
                }

                dto.CriminalRecordFilePath = upload.FilePath;
            }

            // Final Register
            var result = await _authService.RegisterCraftsmanAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("sanaiyCookie");
            return RedirectToAction("Login", "UserAuth");
        }
    }
}
