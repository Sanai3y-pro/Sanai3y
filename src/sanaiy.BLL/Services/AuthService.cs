using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using BCrypt.Net;
using sanaiy.BLL.DTOs.Auth;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
// using sanaiy.BLL.Interfaces.Services; // Uncomment if interfaces are in a subfolder

namespace sanaiy.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        // ==========================================================
        // 1. Client Login
        // ==========================================================
        public async Task<ResultDto<LoginResponseDto>> LoginClientAsync(LoginDto loginDto)
        {
            try
            {
                var client = await _unitOfWork.Clients.SingleOrDefaultAsync(c => c.Email == loginDto.Email);

                if (client == null)
                    return ResultDto<LoginResponseDto>.FailureResult("Invalid email or password.");

                if (!VerifyPassword(loginDto.Password, client.Password))
                    return ResultDto<LoginResponseDto>.FailureResult("Invalid email or password.");

                if (!client.IsActive)
                    return ResultDto<LoginResponseDto>.FailureResult("This account is inactive. Please contact support.");

                var response = new LoginResponseDto
                {
                    UserId = client.Id,
                    FullName = client.FullName,
                    Email = client.Email,
                    UserType = "Client",
                    IsVerified = true,
                    ProfileImageUrl = client.ProfileImageUrl
                };

                return ResultDto<LoginResponseDto>.SuccessResult(response, "Login successful.");
            }
            catch (Exception ex)
            {
                return ResultDto<LoginResponseDto>.FailureResult("An error occurred during login.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // 2. Craftsman Login
        // ==========================================================
        public async Task<ResultDto<LoginResponseDto>> LoginCraftsmanAsync(LoginDto loginDto)
        {
            try
            {
                var craftsman = await _unitOfWork.Craftsmen.SingleOrDefaultAsync(c => c.Email == loginDto.Email);

                if (craftsman == null)
                    return ResultDto<LoginResponseDto>.FailureResult("Invalid email or password.");

                if (!VerifyPassword(loginDto.Password, craftsman.Password))
                    return ResultDto<LoginResponseDto>.FailureResult("Invalid email or password.");

                // Status Checks
                if (craftsman.Status == CraftsmanApplicationStatus.Rejected)
                    return ResultDto<LoginResponseDto>.FailureResult("Your application has been rejected.");

                if (craftsman.Status == CraftsmanApplicationStatus.Suspended)
                    return ResultDto<LoginResponseDto>.FailureResult("Your account has been suspended.");

                var response = new LoginResponseDto
                {
                    UserId = craftsman.Id,
                    FullName = craftsman.FullName,
                    Email = craftsman.Email,
                    UserType = "Craftsman",
                    IsVerified = craftsman.IsVerified, // Frontend uses this to restrict access if false
                    ProfileImageUrl = craftsman.ProfileImageUrl
                };

                return ResultDto<LoginResponseDto>.SuccessResult(response, "Login successful.");
            }
            catch (Exception ex)
            {
                return ResultDto<LoginResponseDto>.FailureResult("An error occurred during login.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // 3. Register Client
        // ==========================================================
        public async Task<ResultDto<Guid>> RegisterClientAsync(RegisterClientDto registerDto)
        {
            try
            {
                // Check if email already exists
                var existingClient = await _unitOfWork.Clients.SingleOrDefaultAsync(c => c.Email == registerDto.Email);
                if (existingClient != null)
                    return ResultDto<Guid>.FailureResult("Email is already registered.");

                // Map DTO to Entity
                var client = _mapper.Map<Client>(registerDto);

                // Hash Password & Set Status
                client.Password = HashPassword(registerDto.Password);
                client.Status = UserStatus.Active;
                client.IsActive = true;

                client.ProfileImageUrl = registerDto.ProfileImageUrl;

                // Save
                _unitOfWork.Clients.Add(client);
                await _unitOfWork.CompleteAsync();

                // ===========================
                // 🔵 Create default address
                // ===========================
                var address = new Address
                {
                    ClientId = client.Id,
                    City = registerDto.City,
                    FullAddress = registerDto.FullAddress,
                    AddressType = AddressType.Home,
                    IsDefault = true
                };

                _unitOfWork.Addresses.Add(address);
                await _unitOfWork.CompleteAsync();

                // Send Welcome Email (Optional - Wrapped in try/catch to avoid breaking flow)
                try
                {
                    await _emailService.SendWelcomeEmailAsync(client.Email, client.FullName);
                }
                catch { /* Log email failure but don't stop registration */ }

                return ResultDto<Guid>.SuccessResult(client.Id, "Registration successful.");
            }
            catch (Exception ex)
            {
                return ResultDto<Guid>.FailureResult("An error occurred during registration.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // 4. Register Craftsman
        // ==========================================================
        public async Task<ResultDto<Guid>> RegisterCraftsmanAsync(RegisterCraftsmanDto registerDto)
        {
            try
            {
                // Check if email already exists
                var existingCraftsman = await _unitOfWork.Craftsmen.SingleOrDefaultAsync(c => c.Email == registerDto.Email);
                if (existingCraftsman != null)
                    return ResultDto<Guid>.FailureResult("Email is already registered.");

                // Map DTO to Entity
                var craftsman = _mapper.Map<Craftsman>(registerDto);

                // Hash Password & Set Status
                craftsman.Password = HashPassword(registerDto.Password);
                craftsman.Status = CraftsmanApplicationStatus.PendingApproval;
                craftsman.IsVerified = false;

                craftsman.ProfileImageUrl = registerDto.ProfileImageUrl;

                // Add Craftsman
                _unitOfWork.Craftsmen.Add(craftsman);
                // Add default address for craftsman
                var address = new Address
                {
                    CraftsmanId = craftsman.Id,
                    City = registerDto.City,
                    FullAddress = registerDto.FullAddress,
                    AddressType = AddressType.Home,
                    IsDefault = true
                };

                _unitOfWork.Addresses.Add(address);

                // Add wallet

                // *** CRITICAL BUSINESS LOGIC: Create Empty Wallet ***
                var wallet = new Wallet
                {
                    CraftsmanId = craftsman.Id,
                    Balance = 0,
                    IsFrozen = false,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.Wallets.Add(wallet);

                await _unitOfWork.CompleteAsync();

                // Send Welcome Email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(craftsman.Email, craftsman.FullName);
                }
                catch { /* Log error */ }

                return ResultDto<Guid>.SuccessResult(craftsman.Id, "Application submitted successfully. Please wait for approval.");
            }
            catch (Exception ex)
            {
                return ResultDto<Guid>.FailureResult("An error occurred during registration.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // 5. Change Password
        // ==========================================================
        public async Task<ResultDto> ChangePasswordAsync(Guid userId, string userType, string oldPassword, string newPassword)
        {
            try
            {
                if (userType == "Client")
                {
                    var client = await _unitOfWork.Clients.GetByIdAsync(userId);
                    if (client == null) return ResultDto.FailureResult("User not found.");

                    if (!VerifyPassword(oldPassword, client.Password))
                        return ResultDto.FailureResult("Incorrect old password.");

                    client.Password = HashPassword(newPassword);
                    _unitOfWork.Clients.Update(client);
                }
                else if (userType == "Craftsman")
                {
                    var craftsman = await _unitOfWork.Craftsmen.GetByIdAsync(userId);
                    if (craftsman == null) return ResultDto.FailureResult("User not found.");

                    if (!VerifyPassword(oldPassword, craftsman.Password))
                        return ResultDto.FailureResult("Incorrect old password.");

                    craftsman.Password = HashPassword(newPassword);
                    _unitOfWork.Craftsmen.Update(craftsman);
                }

                await _unitOfWork.CompleteAsync();
                return ResultDto.SuccessResult("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return ResultDto.FailureResult("An error occurred while changing password.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // 6. Reset Password (Stub)
        // ==========================================================
        public async Task<ResultDto> ResetPasswordAsync(string email, string userType)
        {
            try
            {
                // Logic: Generate Token -> Save to DB -> Send Email
                return await Task.FromResult(ResultDto.SuccessResult("A password reset link has been sent to your email."));
            }
            catch (Exception ex)
            {
                return ResultDto.FailureResult("An error occurred.", new List<string> { ex.Message });
            }
        }

        // ==========================================================
        // Helpers
        // ==========================================================
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
