using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using sanaiy.BLL.DTOs.Address;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;
using sanaiy.BLL.Interfaces;

namespace sanaiy.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب كل عناوين الـ Client
        /// </summary>
        public async Task<ResultDto<IEnumerable<AddressListItemDto>>> GetUserAddressesAsync(Guid userId, string userType)
        {
            try
            {
                // Get addresses for client only
                var addresses = await _unitOfWork.Addresses.FindAsync(a => a.ClientId == userId);

                // Order by IsDefault first, then by CreatedAt
                var orderedAddresses = addresses
                    .OrderByDescending(a => a.IsDefault)
                    .ThenByDescending(a => a.CreatedAt)
                    .ToList();

                var addressDtos = _mapper.Map<IEnumerable<AddressListItemDto>>(orderedAddresses);

                return new ResultDto<IEnumerable<AddressListItemDto>>
                {
                    Success = true,
                    Data = addressDtos
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<AddressListItemDto>>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب العناوين",
                    Errors = new() { ex.Message }
                };
            }
        }

        /// <summary>
        /// إضافة عنوان جديد للـ Client
        /// </summary>
        public async Task<ResultDto<Guid>> CreateAddressAsync(Guid userId, string userType, CreateAddressDto createDto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(createDto.FullAddress))
                {
                    return new ResultDto<Guid> { Success = false, Message = "العنوان مطلوب" };
                }

                // Check if client exists
                var client = await _unitOfWork.Clients.GetByIdAsync(userId);
                if (client == null)
                    return new ResultDto<Guid> { Success = false, Message = "العميل غير موجود" };

                // If this is set as default, remove default from other addresses
                if (createDto.IsDefault)
                {
                    var existingAddresses = await _unitOfWork.Addresses.FindAsync(a => a.ClientId == userId);

                    foreach (var addr in existingAddresses)
                    {
                        addr.IsDefault = false;
                        _unitOfWork.Addresses.Update(addr);
                    }
                }

                // Create new address
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    FullAddress = createDto.FullAddress,
                    City = createDto.City,
                    IsDefault = createDto.IsDefault,
                    ClientId = userId,
                    CraftsmanId = null
                };

                // Parse AddressType enum
                if (!string.IsNullOrEmpty(createDto.AddressType))
                {
                    if (Enum.TryParse<AddressType>(createDto.AddressType, out var addressType))
                    {
                        address.AddressType = addressType;
                    }
                }

                _unitOfWork.Addresses.Add(address);
                await _unitOfWork.CompleteAsync();

                return new ResultDto<Guid>
                {
                    Success = true,
                    Message = "تم إضافة العنوان بنجاح",
                    Data = address.Id
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<Guid>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إضافة العنوان",
                    Errors = new() { ex.Message }
                };
            }
        }

        // Empty implementations for interface requirements (not used)
        public Task<ResultDto> UpdateAddressAsync(Guid userId, UpdateAddressDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> DeleteAddressAsync(Guid addressId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> SetDefaultAddressAsync(Guid addressId, Guid userId)
        {
            throw new NotImplementedException();
        }
        public async Task<ResultDto<AddressListItemDto>> GetAddressByIdAsync(Guid addressId)
        {
            try
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

                if (address == null)
                {
                    return new ResultDto<AddressListItemDto>
                    {
                        Success = false,
                        Message = "العنوان غير موجود"
                    };
                }

                var dto = _mapper.Map<AddressListItemDto>(address);

                return new ResultDto<AddressListItemDto>
                {
                    Success = true,
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<AddressListItemDto>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب العنوان",
                    Errors = new() { ex.Message }
                };
            }
        }

    }
}