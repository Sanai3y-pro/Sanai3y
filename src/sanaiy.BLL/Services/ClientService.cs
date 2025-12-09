using AutoMapper;
using sanaiy.BLL.DTOs.Client;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sanaiy.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ClientService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<ResultDto<ClientProfileDto>> GetProfileAsync(Guid clientId)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetWithIncludesAsync(
                   c => c.Id == clientId,
                   c => c.Bookings,
                   c => c.Addresses  
                );

                if (!client.Any())
                    return new ResultDto<ClientProfileDto> { Success = false, Message = "العميل غير موجود" };

                var clientEntity = client.First();
                var profileDto = _mapper.Map<ClientProfileDto>(clientEntity);
                var addressEntity = clientEntity.Addresses.FirstOrDefault();

                profileDto.Location = addressEntity?.City ?? "N/A";

                return new ResultDto<ClientProfileDto> { Success = true, Data = profileDto };
            }
            catch (Exception ex)
            {
                return new ResultDto<ClientProfileDto> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> UpdateProfileAsync(Guid clientId, UpdateClientProfileDto updateDto)
        {
            try
            {
                // Load client with addresses
                var clientList = await _unitOfWork.Clients.GetWithIncludesAsync(
                    c => c.Id == clientId,
                    c => c.Addresses
                );

                var client = clientList.FirstOrDefault();
                if (client == null)
                return new ResultDto { Success = false, Message = "العميل غير موجود" };

                // Update basic info
                client.Fname = updateDto.FName;
                client.Lname = updateDto.LName;
                client.Phone = updateDto.Phone;


                // ================================
                // 🔵 UPDATE CITY HERE
                // ================================
                var address = client.Addresses.FirstOrDefault(a => a.IsDefault);

                if (address != null)
                {
                    // Update
                    address.City = updateDto.City ?? address.City;
                    address.FullAddress = updateDto.City ?? address.FullAddress;
                    address.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.Addresses.Update(address);
                }
                else
                {
                    // Insert new default address
                    var newAddress = new Address
                    {
                        Id = Guid.NewGuid(),
                        ClientId = clientId,
                        City = updateDto.City ?? "Unknown",
                        FullAddress = updateDto.City ?? "Unknown",
                        AddressType = AddressType.Home,
                        IsDefault = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _unitOfWork.Addresses.Add(newAddress);
                }
                await _unitOfWork.CompleteAsync();


                // Handle profile image if provided
                if (!string.IsNullOrEmpty(updateDto.ProfileImagePath))
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(client.ProfileImageUrl))
                        await _fileService.DeleteFileAsync(client.ProfileImageUrl);

                    client.ProfileImageUrl = updateDto.ProfileImagePath;
                }

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم تحديث الملف الشخصي بنجاح" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ أثناء التحديث", Errors = new() { ex.Message } };
            }
        }
        private string HashPassword(string password)
        {
            // This is a simple example; use a secure hashing algorithm like BCrypt
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<ResultDto<IEnumerable<ClientListItemDto>>> GetAllClientsAsync()
        {
            try
            {
                var clients = await _unitOfWork.Clients.GetWithIncludesAsync(
                    predicate: null,
                    c => c.Bookings
                );

                var clientDtos = _mapper.Map<IEnumerable<ClientListItemDto>>(clients);

                return new ResultDto<IEnumerable<ClientListItemDto>> { Success = true, Data = clientDtos };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<ClientListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto<PaginatedResultDto<ClientListItemDto>>> GetClientsPaginatedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (clients, totalCount) = await _unitOfWork.Clients.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    predicate: null,
                    orderBy: c => c.CreatedAt,
                    ascending: false
                );

                var clientDtos = _mapper.Map<IEnumerable<ClientListItemDto>>(clients);

                var result = new PaginatedResultDto<ClientListItemDto>
                {
                    Items = clientDtos,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return new ResultDto<PaginatedResultDto<ClientListItemDto>> { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new ResultDto<PaginatedResultDto<ClientListItemDto>> { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> DeactivateClientAsync(Guid clientId)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                if (client == null)
                    return new ResultDto { Success = false, Message = "العميل غير موجود" };

                client.IsActive = false;
                client.Status = UserStatus.Suspended;

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم تعطيل حساب العميل" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> ActivateClientAsync(Guid clientId)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                if (client == null)
                    return new ResultDto { Success = false, Message = "العميل غير موجود" };

                client.IsActive = true;
                client.Status = UserStatus.Active;

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم تفعيل حساب العميل" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ", Errors = new() { ex.Message } };
            }
        }
         public async Task<ResultDto> ChangePasswordAsync(Guid clientId, string currentPassword, string newPassword)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                if (client == null)
                    return new ResultDto { Success = false, Message = "العميل غير موجود" };

                // تحقق من الباسورد الحالي
                if (client.Password != HashPassword(currentPassword))
                    return new ResultDto { Success = false, Message = "كلمة المرور الحالية غير صحيحة" };

                // تحديث الباسورد الجديد
                client.Password = HashPassword(newPassword);
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم تغيير كلمة المرور بنجاح" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ أثناء تغيير كلمة المرور", Errors = new() { ex.Message } };
            }
        }

        public async Task<ResultDto> DeleteAccountAsync(Guid clientId, string password)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                if (client == null)
                    return new ResultDto { Success = false, Message = "العميل غير موجود" };

                // تحقق من الباسورد
                if (client.Password != HashPassword(password))
                    return new ResultDto { Success = false, Message = "كلمة المرور غير صحيحة" };

                // حذف الحساب
                _unitOfWork.Clients.Remove(client);
                await _unitOfWork.CompleteAsync();

                return new ResultDto { Success = true, Message = "تم حذف الحساب بنجاح" };
            }
            catch (Exception ex)
            {
                return new ResultDto { Success = false, Message = "حدث خطأ أثناء حذف الحساب", Errors = new() { ex.Message } };
            }
        }

    }
}
